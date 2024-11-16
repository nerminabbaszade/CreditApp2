using CreditApp.DAL.DTOs.Account;

namespace CreditApp.DAL.DTOs.Customers;

public class CustomerUpdate : UpdateUser
{
    public string Occupation { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
}