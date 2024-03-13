using TheaAdmin.Domain;

namespace TheaAdmin.Dtos;

public class MemberRequest
{
    public string MemberId { get; set; }
    public string MemberName { get; set; }
    public string Mobile { get; set; }
    public string Description { get; set; }
    public Gender Gender { get; set; }
    public double Balance { get; set; }
}
