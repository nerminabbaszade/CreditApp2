using CreditApp.DAL.DTOs.Account;

namespace CreditApp.DAL.DTOs.Merchants;

public class MerchantPost :Register
{
    public string Description  {get; set;}
    public string TerminalNo {get; set;}
}