using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RepairTracking.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Get the platform-specific folder for application data.
        // Using a generic path here is fine for the tools, 
        // as they just need to build the model.
        // The actual application will use the correct user-specific path at runtime.
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var dbFolder = Path.Combine(appDataFolder, "RepairTracking");
        var dbPath = Path.Combine(dbFolder, "app.db");

        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        return new AppDbContext(optionsBuilder.Options);
    }
}