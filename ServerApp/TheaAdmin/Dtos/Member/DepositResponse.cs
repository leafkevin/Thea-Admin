using System;

namespace TheaAdmin.Dtos;

public class DepositResponse
{
    public string DepositId { get; set; }
    public string MemberId { get; set; }
    public string MemberName { get; set; }
    public string Mobile { get; set; }
    public double Amount { get; set; }
    public double Bonus { get; set; }
    public double EndBalance { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsAllowCancel { get; set; }
}
