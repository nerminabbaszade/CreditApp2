using CreditApp.DAL.DTOs.Account;

namespace CreditApp.DAL.DTOs.Employees;

public class EmployeePost : Register
{
    public string Position {get; set;}
    public string BranchId {get; set;}
}