using Thea;

namespace TheaAdmin.Dtos;

public class MemberQueryRequest : QueryRequest
{
    public string MemberName { get; set; }
    public string Mobile { get; set; }
}
