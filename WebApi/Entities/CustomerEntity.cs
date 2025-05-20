namespace WebApi.Entities;



// Extract to its own service?
public class CustomerEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string StreetAddress { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string? Phone { get; set; }
    public string Email { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
}
