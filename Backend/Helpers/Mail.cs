using System.Net;
using System.Net.Mail;

namespace Backend.Helpers;

public static class Mail
{
    private static IConfiguration _config;

    public static void Init(IConfiguration config)
    {
        _config = config;
    }
    
    public static string Sent2FA(string address, string code)
    {
        try
        {
            var fromAddress = new MailAddress(_config["AppSettings:Mail:Username"], "Admin");
            var toAddress = new MailAddress(address);
            string fromPassword = _config["AppSettings:Mail:Password"];

            var smtp = new SmtpClient
            {
                Host = _config["appSettings:Mail:Host"],
                Port = int.Parse(_config["appSettings:Mail:Port"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "Login to Admin Panel - Solzycia.pl",
                Body = "Your 2FA code is: " + code +
                       "\n\nThis code will expire in 15 minutes.\n\nIf you didn't request this code, change password as fast as you can!!!"
            })
            {
                smtp.Send(message);
            }

            return "SUCCESS";
        } catch (Exception e)
        {
            return "ERROR " + e.Message;
        }
    }
}