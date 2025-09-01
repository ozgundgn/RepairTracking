using System;

namespace RepairTracking.Models;

public class UserInfo
{
    public Guid GuidId { get; set; }
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string Fullname => $"{Name} {Surname}";
}