using System;
using System.Collections.Generic;

namespace RepairTracking.Data.Models;

public partial class RenovationDetail
{
    public string? Description { get; set; }

    public string? Name { get; set; }

    public double Price { get; set; }

    public int? TCode { get; set; }

    public int? Note { get; set; }

    public int Id { get; set; }

    public int? RenovationId { get; set; }
}
