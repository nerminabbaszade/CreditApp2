using System.Net;
using System.Net.Mail;
using CreditApp.BLL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CreditApp.BLL.Services;

public class MailService : IMailService
{
    private readonly IConfiguration _configuration;

    public MailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendMail(string to, string subject, string body)
    {
        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(_configuration.GetSection("MailConfig:Email").Value);
        mailMessage.To.Add(to);
        mailMessage.Subject = subject;
        mailMessage.Body = body;

        SmtpClient smtpClient = new SmtpClient();
        smtpClient.Host = "smtp.gmail.com";
        smtpClient.Port = 587;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential(_configuration.GetSection("MailConfig:Email").Value, _configuration.GetSection("MailConfig:Password").Value);
        smtpClient.EnableSsl = true;

        try
        {
            smtpClient.Send(mailMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}