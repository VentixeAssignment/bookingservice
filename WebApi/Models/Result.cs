namespace WebApi.Models;

public class Result<T> where T : class
{
    public bool Success { get; set; }
    public T? Data { get; set; } 
    public IEnumerable<T>? DataList { get; set; }
    public string? ErrorMessage { get; set; }
    public int? StatusCode { get; set; }
}
