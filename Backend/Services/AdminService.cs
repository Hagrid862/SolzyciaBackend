using Backend.Data;
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
    
    public async Task<string> Login(string userName, string password)
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

            var TwoFactorRequest = new TwoFactorAuth
            {
                Id = Helpers.Snowflake.GenerateId(),
                Code = code.ToString(),
                Admin = null,
                CreatedAt = default
            };
            
            _context.TwoFactorAuths.Add(TwoFactorRequest);
            return "SUCCESS";
        } catch (Exception e)
        {
            return "ERROR";
        }
    }
}

public interface IAdminService
{
    Task<string> Login(string userName, string password);
}