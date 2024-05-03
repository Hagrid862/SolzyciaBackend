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
    
    public async Task<string> Login(string userName, string password, bool remember)
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
                Admin = admin,
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                Remember = remember
            };

            await _context.TwoFactorAuths.AddAsync(twoFactorRequest);
            await _context.SaveChangesAsync();
            return "SUCCESS";
        } catch (Exception e)
        {
            return "ERROR";
        }
    }
    
    public async Task<(string access, string? refresh)> Verify(string username, string password, string code)
    {
        try
        {
            //get latest code for username
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Name == username);
            if (admin == null)
                return ( "NOTFOUND", null);

            var passwordHash = Helpers.Hasher.GetHash(password, admin.PasswordSalt);
            if (passwordHash != admin.PasswordHash)
                return ("INVALID", null);

            var twoFactorRequest = await _context.TwoFactorAuths
                .Where(t => t.Admin == admin)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
            
            if (twoFactorRequest == null)
                return ("NOTFOUND", null);

            if (twoFactorRequest.Code != code)
                return ("INVALID", null);
            
            if (twoFactorRequest.IsUsed)
                return ("INVALID", null);

            if (DateTime.UtcNow - twoFactorRequest.CreatedAt > TimeSpan.FromMinutes(15))
                return ("EXPIRED", null);


            if (twoFactorRequest.Remember)
            {
                var (refresh, access) = JWT.GenerateLoginTermAdminToken(admin);
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
        } catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return ("INTERNALERROR", "");
        }
    }
}

public interface IAdminService
{
    Task<string> Login(string userName, string password, bool remember);
    Task<(string access, string refresh)> Verify(string username, string password, string code);
}