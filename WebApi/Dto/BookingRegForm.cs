using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto;

public class BookingRegForm : IValidatableObject
{
    [Required(ErrorMessage = "Field is required.")]
    public int Seats { get; set; } = 1;


    [Required(ErrorMessage = "Field is required.")]
    public decimal TotalPrice { get; set; }


    [Required]
    public string EventId { get; set; } = null!;


    [Range(typeof(bool), "true", "true", ErrorMessage = "You have to accept the terms and conditions.")]
    public bool TermsAndConditions { get; set; }


    // CustomerId or CustomerName and CustomerEmail must be assigned to book an event.
    public string? CustomerId { get; set; }


    [StringLength(20, MinimumLength = 2, ErrorMessage = "First name must be at least 2 characters long.")]
    public string? CustomerFirstName { get; set; }


    [StringLength(30, MinimumLength = 2, ErrorMessage = "Last name must be at least 2 characters long.")]
    public string? CustomerLastName { get; set; }


    [StringLength(250, MinimumLength = 6, ErrorMessage = "Email must be at least 6 characters long.")]
    [RegularExpression(@"^(?i)([a-z0-9._%+-]+)@([a-z0-9.-]+\.[a-z]{2,})$", ErrorMessage = "Invalid email address.")]
    public string? CustomerEmail { get; set; }


    [StringLength(20, MinimumLength = 5, ErrorMessage = "Phone number be at least 7 characters long.")]
    [RegularExpression(@"^\+?[0-9\s\-\(\)]{7,20}$")]
    public string? CustomerPhone { get; set; }


    public DateOnly? CustomerBirthDate { get; set; }


    [StringLength(50, MinimumLength = 2, ErrorMessage = "Sreet address must be at least 2 characters long.")]
    public string? CustomerStreetAddress { get; set; }


    [StringLength(10, MinimumLength = 2, ErrorMessage = "Postal code must be at least 3 characters long.")]
    [RegularExpression(@"^\d{3,10}$")]
    public string? CustomerPostalCode { get; set; }


    [StringLength(30, MinimumLength = 2, ErrorMessage = "City must be at least 2 characters long.")]
    public string? CustomerCity { get; set; }


    // Validate that customer information is added.
    // This method is created by consulting ChatGpt to ensure that there is either a logged in user or that the customer details is entered manually.
    // The method is used because all customer fields is nullable and I still want to have validation so there have to be some kind of customer data in the form.
    // Also added validation for each field (not with help of ChatGpt).
    // Using yield so that a list doesn't have to be saved in memory.
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        bool customerId = !string.IsNullOrWhiteSpace(CustomerId);

        bool customerDetails = !string.IsNullOrWhiteSpace(CustomerFirstName)
                                && !string.IsNullOrWhiteSpace(CustomerLastName)
                                && !string.IsNullOrWhiteSpace(CustomerEmail);
        if(!customerId) 
        {
            if(string.IsNullOrWhiteSpace(CustomerFirstName))
                yield return new ValidationResult( "Field is required.", [nameof(CustomerFirstName)] );

            if (string.IsNullOrWhiteSpace(CustomerLastName))
                yield return new ValidationResult("Field is required.", [nameof(CustomerLastName)]);

            if (string.IsNullOrWhiteSpace(CustomerEmail))
                yield return new ValidationResult( "Field is required.", [nameof(CustomerEmail)] );
        }
    }
}
