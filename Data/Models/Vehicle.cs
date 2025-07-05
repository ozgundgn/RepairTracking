using System;
using System.Collections.Generic;

namespace RepairTracking.Data.Models;

public partial class Vehicle
{
    public string PlateNumber { get; set; } = null!;

    public string? ChassisNo { get; set; }

    public int CustomerId { get; set; }

    public int? Model { get; set; }

    public string? Color { get; set; }

    public string? Type { get; set; }

    public string? EngineNo { get; set; }

    public int? Km { get; set; }

    public string? Fuel { get; set; }

    public int Id { get; set; }

    public bool Passive { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<CustomersVehicle> CustomersVehicles { get; set; } = new List<CustomersVehicle>();

    public virtual ICollection<Renovation> Renovations { get; set; } = new List<Renovation>();
}
