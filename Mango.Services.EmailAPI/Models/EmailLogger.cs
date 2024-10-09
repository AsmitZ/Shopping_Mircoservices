namespace Mango.Services.EmailAPI.Models;

public class EmailLogger
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Message { get; set; }
    public DateTime? Timestamp { get; set; }
}