namespace WebApi.Entities;

public class BookingEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int Seats { get; set; } 
    public decimal TotalPrice { get; set; }
    public DateTime Created { get; set; }
    public string CustomerId { get; set; } = null!;
    public string EventId { get; set; } = null!;

    public CustomerEntity Customer { get; set; } = new();
}
