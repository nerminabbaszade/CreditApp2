using System.ComponentModel.DataAnnotations;

namespace CreditApp.DAL.DTOs.Payment;

public class PaymentPost
{
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, ErrorMessage = "Full name can't be longer than 100 characters")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Card number is required")]
    [StringLength(16, MinimumLength = 16, ErrorMessage = "Card number must be 16 characters long")]
    [RegularExpression(@"^\d{16}$", ErrorMessage = "Card number must be exactly 16 digits")]
    public string CardNumber { get; set; }

    [Required(ErrorMessage = "Expiration date is required")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Expiration date must be in MM/YY format")]
    public string ExpirationDate { get; set; }

    [Required(ErrorMessage = "CVV code is required")]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV code must be 3 or 4 digits")]
    public string CvvCode { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public double Amount { get; set; }
}