using System;
using System.Collections.Generic;

namespace RepairTracking.Data.Models;

public partial class Renovation
{
    public DateOnly RepairDate { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public int? VehicleId { get; set; }

    public int Id { get; set; }

    public string? Complaint { get; set; }

    public string? Note { get; set; }

    public virtual Vehicle? Vehicle { get; set; }
}
