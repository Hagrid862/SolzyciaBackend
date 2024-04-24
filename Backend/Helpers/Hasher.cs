using System.Security.Cryptography;
using System.Text;

namespace Backend.Helpers;

public static class Hasher
{
    public static string GetSalt()
    {
        var salt = new byte[128 / 8];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return Convert.ToBase64String(salt);
    }
    
    public static string GetHash(string password, string salt)
    {
        var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 10000);
        return Convert.ToBase64String(pbkdf2.GetBytes(256 / 8));
    }
    
    public static bool Verify(string password, string salt, string hash)
    {
        return GetHash(password, salt) == hash;
    }

    public static string GenerateSHA256String(string inputString)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputString));

            // Convert byte array to a string
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}