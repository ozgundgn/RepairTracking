using System;
using System.Collections.Generic;

namespace RepairTracking.Data.Models;

public partial class RenovationDetail
{
    public string? Description { get; set; }

    public string? Name { get; set; }

    public double Price { get; set; }

    public int? TCode { get; set; }

    public string? Note { get; set; }

    public int Id { get; set; }

    public int RenovationId { get; set; }

    public int? Passive { get; set; }

    public virtual Renovation Renovation { get; set; } = null!;
}
