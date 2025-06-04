using System;
using System.Collections.Generic;

namespace RepairTracking.Data.Models;

public partial class User
{
    public Guid UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Id { get; set; }

    public bool Passive { get; set; }
}
