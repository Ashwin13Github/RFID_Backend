using System.Net;
using System.Net.Mail;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtp = _config.GetSection("Smtp");

        var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"]))
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(smtp["UserName"], smtp["Password"])
        };

        var mail = new MailMessage
        {
            From = new MailAddress(smtp["UserName"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mail.To.Add(to);

        await client.SendMailAsync(mail);
    }
}
