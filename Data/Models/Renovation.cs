using System;
using System.Collections.Generic;

namespace RepairTracking.Data.Models;

public partial class Renovation
{
    public DateOnly RepairDate { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public int VehicleId { get; set; }

    public int Id { get; set; }

    public string? Complaint { get; set; }

    public string? Note { get; set; }

    public bool? Passive { get; set; }

    public string? ReportPath { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<RenovationDetail> RenovationDetails { get; set; } = new List<RenovationDetail>();

    public virtual Vehicle Vehicle { get; set; } = null!;
}
