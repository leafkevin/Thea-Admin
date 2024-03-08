namespace TheaAdmin.Dtos;

public class CreateOrderRequest
{
    public string MemberId { get; set; }
    public string StylistId { get; set; }
    public double Amount { get; set; }
    public bool IsAppointed { get; set; }
    public string Description { get; set; }
}
