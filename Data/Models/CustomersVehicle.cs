using System;
using System.Collections.Generic;

namespace RepairTracking.Data.Models;

public partial class CustomersVehicle
{
    public int CustomerId { get; set; }

    public int VehicleId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int Id { get; set; }

    public bool Passive { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
