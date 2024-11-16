using System.ComponentModel.DataAnnotations;

namespace CreditApp.DAL.DTOs.Account;

public class ResetPassword
{
    public string Mail { get; set; }
    public string Token { get; set; }
    public string Password { get; set; }
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}