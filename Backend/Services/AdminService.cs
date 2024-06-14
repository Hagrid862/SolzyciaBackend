using Backend.Data;
using Backend.Helpers;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class AdminService : IAdminService
{
    private readonly MainDbContext _context;

    public AdminService(MainDbContext context)
    {
        _context = context;
    }

    public async Task<string> Login(string userName, string password, bool remember, string clientIp)
    {
        try
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Name == userName);
            if (admin == null)
                return "NOTFOUND";

            var passwordHash = Helpers.Hasher.GetHash(password, admin.PasswordSalt);
            if (passwordHash != admin.PasswordHash)
                return ("INVALID");

            var code = new Random().Next(10000000, 99999999).ToString();

            var sentStatus = Helpers.Mail.Sent2FA(admin.Email, code);

            if (sentStatus != "SUCCESS")
                return "ERROR";

            var twoFactorRequest = new TwoFactorAuth
            {
                Id = Helpers.Snowflake.GenerateId(),
                Code = code,
                IP = clientIp,
                Admin = admin,
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                Remember = remember
            };

            await _context.TwoFactorAuths.AddAsync(twoFactorRequest);
            await _context.SaveChangesAsync();
            return "SUCCESS";
        }
        catch (Exception e)
        {
            return "ERROR";
        }
    }

    public async Task<(string access, string? refresh)> VerifyOtp(string code, string clientIp)
    {
        try
        {
            var twoFactorRequest = await _context.TwoFactorAuths.Include(twoFactorAuth => twoFactorAuth.Admin).FirstOrDefaultAsync(r => r.IP == clientIp && r.Code == code);

            if (twoFactorRequest == null)
                return ("NOTFOUND", null);

            if (twoFactorRequest.IsUsed)
                return ("INVALID", null);

            if (DateTime.UtcNow - twoFactorRequest.CreatedAt > TimeSpan.FromMinutes(15))
                return ("EXPIRED", null);

            var admin = twoFactorRequest.Admin;

            if (twoFactorRequest.Remember)
            {
                var (refresh, access) = JWT.GenerateLongTermAdminToken(admin);
                if (refresh == "NOTINITIALIZED" || access == "NOTINITIALIZED" || refresh == "ERROR" || access == "ERROR")
                    return ("INTERNALERROR", null);

                twoFactorRequest.IsUsed = true;
                _context.TwoFactorAuths.Update(twoFactorRequest);
                await _context.SaveChangesAsync();
                return (access, refresh);
            }
            else
            {
                var token = JWT.GenerateAdminToken(admin);
                if (token == "NOTINITIALIZED" || token == "ERROR")
                    return ("INTERNALERROR", "");

                twoFactorRequest.IsUsed = true;
                _context.TwoFactorAuths.Update(twoFactorRequest);
                await _context.SaveChangesAsync();
                return (token, null);
            }
        }
        catch (Exception e)
        {
            return ("INTERNALERROR", "");
        }
    }

    public async Task<(bool exists, bool valid)> VerifyToken(string token)
    {
        try
        {
            return JWT.IsValid(token);
        }
        catch (Exception e)
        {
            return (false, false);
        }
    }

    public async Task<(string access, string refresh)> RefreshToken(string refreshToken)
    {
        try
        {
            var (access, refresh) = JWT.RefreshToken(refreshToken);

            if (access == "NOTINITIALIZED" || refresh == "NOTINITIALIZED" || access == "ERROR" || refresh == "ERROR")
                return ("INTERNALERROR", "");

            return (access, refresh);
        }
        catch (Exception e)
        {
            return ("INTERNALERROR", "");
        }
    }
}

public interface IAdminService
{
    Task<string> Login(string userName, string password, bool remember, string clientIp);
    Task<(string access, string? refresh)> VerifyOtp(string code, string clientIp);
    Task<(bool exists, bool valid)> VerifyToken(string token);
    Task<(string access, string refresh)> RefreshToken(string refresh);
}