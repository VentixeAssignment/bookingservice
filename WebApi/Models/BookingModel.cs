using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class BookingModel
{
    public string Id { get; set; } = null!;
    public int Seats { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal TotalPrice { get; set; }
    public string EventId { get; set; } = null!;
    public bool TermsAndConditions { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerFirstName { get; set; }
    public string? CustomerLastName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public DateOnly? CustomerBirthDate { get; set; }
    public string? CustomerStreetAddress { get; set; }
    public string? CustomerPostalCode { get; set; }
    public string? CustomerCity { get; set; }
}
