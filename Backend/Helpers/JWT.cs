using System.Security.Claims;
using System.Text;
using Backend.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Helpers;

public static class JWT
{
    private static string secret;

    public static void Init(IConfiguration configuration)
    {
        secret = configuration["AppSettings:JWT:Secret"];
    }
    
    public static string GenerateAdminToken(Admin admin)
    {
        if (secret == null)
            return "NOTINITIALIZED";
        
        var data = Encoding.UTF8.GetBytes(secret);
        var securityKey = new SymmetricSecurityKey(data);

        var claims = new Dictionary<String, object>()
        {
            ["Id"] = admin.Id,
        };
        
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims.Select(claim => new Claim(claim.Key, claim.Value.ToString()))),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var handler = new JsonWebTokenHandler();
        handler.SetDefaultTimesOnTokenCreation = false;
        var token = handler.CreateToken(descriptor);
        if (token == null)
            return null;
        else
            return token.ToString();
    }
}