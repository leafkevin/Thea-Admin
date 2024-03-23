namespace TheaAdmin.Dtos;

public class DepositRequest
{
    public string DepositId { get; set; }
    public string MemberId { get; set; }
    public double Amount { get; set; }
    public double Bonus { get; set; }
    public string Description { get; set; }
}
