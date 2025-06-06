using System;
using System.Collections.Generic;

namespace RepairTracking.Data.Models;

public partial class Customer
{
    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int Id { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public bool Passive { get; set; }

    public virtual ICollection<CustomersVehicle> CustomersVehicles { get; set; } = new List<CustomersVehicle>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
