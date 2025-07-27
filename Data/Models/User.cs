using System;
using System.Collections.Generic;

namespace RepairTracking.Data.Models;

public partial class User
{
    public Guid UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool Passive { get; set; }

    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
