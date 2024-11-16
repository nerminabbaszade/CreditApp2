using System.ComponentModel.DataAnnotations;

namespace CreditApp.DAL.DTOs.Account;

public class ForgetPassword
{
    [EmailAddress]
    public string Mail { get; set; }
}