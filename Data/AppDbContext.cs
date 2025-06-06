using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RepairTracking.Data.Models;

namespace RepairTracking.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomersVehicle> CustomersVehicles { get; set; }

    public virtual DbSet<Renovation> Renovations { get; set; }

    public virtual DbSet<RenovationDetail> RenovationDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk");

            entity.ToTable("customers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Passive).HasColumnName("passive");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.Surname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("surname");
        });

        modelBuilder.Entity<CustomersVehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customers_vehicles_pk");

            entity.ToTable("customers_vehicles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Passive).HasColumnName("passive");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomersVehicles)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customers_vehicles_customers_id_fk");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.CustomersVehicles)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customers_vehicles_vehicles_id_fk");
        });

        modelBuilder.Entity<Renovation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("renovations_pk");

            entity.ToTable("renovations");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Complaint)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("complaint");
            entity.Property(e => e.DeliveryDate)
                .HasColumnType("datetime")
                .HasColumnName("delivery_date");
            entity.Property(e => e.Note)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("note");
            entity.Property(e => e.RepairDate).HasColumnName("repair_date");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Renovations)
                .HasForeignKey(d => d.VehicleId)
                .HasConstraintName("renovations_vehicles_id_fk");
        });

        modelBuilder.Entity<RenovationDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("renovation_details_pk");

            entity.ToTable("renovation_details");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.RenovationId).HasColumnName("renovation_id");
            entity.Property(e => e.TCode).HasColumnName("t-code");

            entity.HasOne(d => d.Renovation).WithMany(p => p.RenovationDetails)
                .HasForeignKey(d => d.RenovationId)
                .HasConstraintName("renovation_details_renovations_id_fk");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pk");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Passive).HasColumnName("passive");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("user_name");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("id");

            entity.ToTable("vehicles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChassisNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("chassis_no");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("color");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.EngineNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("engine_no");
            entity.Property(e => e.Fuel)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("fuel");
            entity.Property(e => e.Km).HasColumnName("km");
            entity.Property(e => e.Model).HasColumnName("model");
            entity.Property(e => e.Passive).HasColumnName("passive");
            entity.Property(e => e.PlateNumber)
                .HasMaxLength(100)
                .HasColumnName("plate_number");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.Customer).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vehicles_customers_id_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
