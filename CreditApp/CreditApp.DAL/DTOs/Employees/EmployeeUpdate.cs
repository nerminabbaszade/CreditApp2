using CreditApp.DAL.DTOs.Account;
using Microsoft.EntityFrameworkCore;

namespace CreditApp.DAL.DTOs.Employees;

public class EmployeeUpdate: UpdateUser
{
    public string Position {get; set;}
    public string BranchId {get; set;}
}