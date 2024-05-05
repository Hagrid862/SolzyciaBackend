using System.Security.Claims;
using System.Text;
using Backend.Data;
using Backend.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Helpers;

public static class JWT
{
    private static MainDbContext context;
    private static string secret;

    public static void Init(IConfiguration configuration, MainDbContext _context)
    {
        secret = configuration["AppSettings:JWT:Secret"];
        context = _context;
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
            return "ERROR";
        else
            return token.ToString();
    }
    
    public static (string refresh, string access) GenerateLoginTermAdminToken(Admin admin)
    {
        if (secret == null)
            return ("NOTINITIALIZED", "NOTINITIALIZED");
        
        var data = Encoding.UTF8.GetBytes(secret);
        var securityKey = new SymmetricSecurityKey(data);

        var accessClaims = new Dictionary<String, object>()
        {
            ["Type"] = "Access",
            ["Id"] = admin.Id,
        };
        
        var refreshClaims = new Dictionary<String, object>()
        {
            ["Type"] = "Refresh",
            ["Id"] = admin.Id,
        };
        
        var accessDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(accessClaims.Select(claim => new Claim(claim.Key, claim.Value.ToString()))),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var refreshDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(refreshClaims.Select(claim => new Claim(claim.Key, claim.Value.ToString()))),
            Expires = DateTime.UtcNow.AddDays(30),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };
        
        var handler = new JsonWebTokenHandler();
        handler.SetDefaultTimesOnTokenCreation = false;
        
        var access = handler.CreateToken(accessDescriptor);
        var refresh = handler.CreateToken(refreshDescriptor);
        
        if (access == null || refresh == null)
            return ("ERROR", "ERROR");
        else
            return (refresh.ToString(), access.ToString());
    }
    
    public static (bool correct, bool valid) IsValid(string token)
    {
        if (secret == null)
            return (false, false);

        var data = Encoding.UTF8.GetBytes(secret);
        var securityKey = new SymmetricSecurityKey(data);

        var handler = new JsonWebTokenHandler();
        try
        {
            var result = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            });

            return (true, true);
        }
        catch (SecurityTokenExpiredException)
        {
            return (true, false);
        }
        catch (Exception)
        {
            return (false, false);
        }
    }

    public static (string access, string? refresh) RefreshToken(string refreshToken)
    {
        if (secret == null)
            return ("NOTINITIALIZED", null);
        
        var data = Encoding.UTF8.GetBytes(secret);
        var securityKey = new SymmetricSecurityKey(data);
        
        var handler = new JsonWebTokenHandler();
        var result = handler.ValidateToken(refreshToken, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        });
        
        if (!result.IsValid)
            return ("INVALID", null);
        
        var claims = result.Claims;
        
        if (claims == null)
            return ("INVALID", null);
        
        var tokenTypeClaim = claims.FirstOrDefault(c => c.Key == "Type");
        if (tokenTypeClaim.Value.ToString() != "Refresh")
            return ("INVALID", null);
        
        var idClaim = claims.FirstOrDefault(c => c.Key == "Id");
        if (idClaim.Value == null)
            return ("INVALID", null);
        
        var admin = context.Admins.FirstOrDefault(a => a.Id == long.Parse(idClaim.Value.ToString()));
        if (admin == null)
            return ("INVALID", null);
        
        var (refresh, access) = GenerateLoginTermAdminToken(admin);
        if (refresh == "NOTINITIALIZED" || access == "NOTINITIALIZED" || refresh == "ERROR" || access == "ERROR")
            return ("INTERNALERROR", null);
        
        return (access, refresh);
    }
}