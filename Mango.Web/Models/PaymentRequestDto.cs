namespace Mango.Web.Models;

public class PaymentRequestDto
{
    public string SessionUrl { get; set; }
    public string SessionId { get; set; }
    public string OnSuccessUrl { get; set; }
    public string OnCancelUrl { get; set; }
    public OrderHeaderDto OrderHeader { get; set; }
}