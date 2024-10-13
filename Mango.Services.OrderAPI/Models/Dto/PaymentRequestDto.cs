namespace Mango.Services.OrderAPI.Models.Dto;

public class PaymentRequestDto
{
    public string SessionUrl { get; set; }
    public string SessionId { get; set; }
    public string OnSuccessUrl { get; set; }
    public string OnCancelUrl { get; set; }
    public OrderHeaderDto OrderHeader { get; set; }
}