using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities;

public class BookingEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public int Seats { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalPrice { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required]
    public string EventId { get; set; } = null!;


    public string? CustomerId { get; set; }

    [MaxLength(20)]
    public string? CustomerFirstName { get; set; }

    [MaxLength(30)]
    public string? CustomerLastName { get; set; }

    [MaxLength(250)]
    [DataType(DataType.EmailAddress)]
    public string? CustomerEmail { get; set; }

    [MaxLength(20)]
    [DataType(DataType.PhoneNumber)]
    public string? CustomerPhone { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? CustomerBirthDate { get; set; }

    [MaxLength(50)] 
    public string? CustomerStreetAddress { get; set; }

    [MaxLength(10)]
    public string? CustomerPostalCode { get; set; }

    [MaxLength(30)]
    public string? CustomerCity { get; set; }
}
