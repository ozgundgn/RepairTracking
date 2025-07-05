using System;

namespace RepairTracking.Models;

public class UserInfo
{
    public Guid GuidId { get; set; }
    public int Id { get; set; }
    public string? Name { get; set; } = null!;
     public string? Surname { get; set; } = null!;
    public string Fullname
    {
        get=> $"{Name} {Surname}";
    }
}