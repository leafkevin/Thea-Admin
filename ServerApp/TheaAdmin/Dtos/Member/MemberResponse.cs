using TheaAdmin.Domain;
using System;

namespace TheaAdmin.Dtos;

public class MemberResponse
{
    public string MemberId { get; set; } 
    public string MemberName { get; set; }  
    public string Description { get; set; } 
    public Gender Gender { get; set; } 
    public string Balance { get; set; } 
    public DateTime CreatedAt { get; set; }
}
