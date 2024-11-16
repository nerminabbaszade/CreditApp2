namespace CreditApp.BLL.Services.Interfaces;

public interface IMailService
{
    void SendMail(string to, string subject, string body);
}