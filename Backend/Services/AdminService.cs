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

    public async Task<(bool isSuccess, string status)> Login(string userName, string password, bool remember, string clientIp)
    {
        try
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Name == userName);
            if (admin == null)
                return (false, "INVALID");

            var passwordHash = Helpers.Hasher.GetHash(password, admin.PasswordSalt);
            if (passwordHash != admin.PasswordHash)
                return (false, "INVALID");

            var code = new Random().Next(10000000, 99999999).ToString();

            var sentStatus = Helpers.Mail.Sent2FA(admin.Email, code);

            if (sentStatus != "SUCCESS")
                return (false, "ERROR");

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
            return (true, "SUCCESS");
        }
        catch
        {
            return (false, "ERROR");
        }
    }

    public async Task<(bool isSuccess, string status, string? access, string? refresh)> VerifyOtp(string code, string clientIp)
    {
        try
        {
            var twoFactorRequest = await _context.TwoFactorAuths.Include(twoFactorAuth => twoFactorAuth.Admin).FirstOrDefaultAsync(r => r.IP == clientIp && r.Code == code);

            if (twoFactorRequest == null)
                return (false, "NOTFOUND", null, null);

            if (twoFactorRequest.IsUsed)
                return (false, "INVALID", null, null);

            if (DateTime.UtcNow - twoFactorRequest.CreatedAt > TimeSpan.FromMinutes(15))
                return (false, "EXPIRED", null, null);

            var admin = twoFactorRequest.Admin;

            if (twoFactorRequest.Remember)
            {
                var (refresh, access) = JWT.GenerateLongTermAdminToken(admin);
                if (refresh == "NOTINITIALIZED" || access == "NOTINITIALIZED" || refresh == "ERROR" || access == "ERROR")
                    return (false, "INTERNALERROR", null, null);

                twoFactorRequest.IsUsed = true;
                _context.TwoFactorAuths.Update(twoFactorRequest);
                await _context.SaveChangesAsync();
                return (true, "SUCCESS", access, refresh);
            }
            else
            {
                var token = JWT.GenerateAdminToken(admin);
                if (token == "NOTINITIALIZED" || token == "ERROR")
                    return (false, "INTERNALERROR", null, null);

                twoFactorRequest.IsUsed = true;
                _context.TwoFactorAuths.Update(twoFactorRequest);
                await _context.SaveChangesAsync();
                return (true, "SUCCESS", token, null);
            }
        }
        catch
        {
            return (false, "INTERNALERROR", null, null);
        }
    }

    public async Task<(bool isSuccess, string status, bool exists, bool valid)> VerifyToken(string token)
    {
        try
        {
            (bool exists, bool valid) = JWT.IsValid(token);

            if (!exists)
                return (true, "SUCCESS", false, false);

            if (exists && !valid)
                return (true, "SUCCESS", true, false);

            if (exists && valid)
                return (true, "SUCCESS", true, true);

            return (false, "ERROR", false, false);
        }
        catch (Exception e)
        {
            return (false, "ERROR", false, false);
        }
    }

    public async Task<(bool isSuccess, string status, string? access, string? refresh)> RefreshToken(string refreshToken)
    {
        try
        {
            var (access, refresh) = JWT.RefreshToken(refreshToken);

            if (access == "NOTINITIALIZED" || refresh == "NOTINITIALIZED" || access == "ERROR" || refresh == "ERROR")
                return (false, "INTERNALERROR", null, null);

            return (true, "SUCCESS", access, refresh);
        }
        catch
        {
            return (true, "INTERNALERROR", null, null);
        }
    }
}

public interface IAdminService
{
    Task<(bool isSuccess, string status)> Login(string userName, string password, bool remember, string clientIp);
    Task<(bool isSuccess, string status, string? access, string? refresh)> VerifyOtp(string code, string clientIp);
    Task<(bool isSuccess, string status, bool exists, bool valid)> VerifyToken(string token);
    Task<(bool isSuccess, string status, string? access, string? refresh)> RefreshToken(string refresh);
}