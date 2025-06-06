using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RepairTracking.Data;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Repositories.Concrete;
using RepairTracking.ViewModels;
using RepairTracking.Views;

namespace RepairTracking;

public class App : Application
{
    public IServiceProvider? Services { get; private set; } // Property to hold the service provider

    public IConfiguration Configuration { get; private set; } // Property to hold the configuration

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
       
        // 1. Configuration (if using appsettings.json)
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Or AppContext.BaseDirectory
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        Configuration = builder.Build();
        // --- Start of DI Configuration ---
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();
        
        // --- End of DI Configuration ---

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            var mainWindow = Services.GetRequiredService<MainWindow>();
            desktop.MainWindow = mainWindow;
            // Option 1: Set DataContext here if not set in MainWindow constructor
            // mainWindow.DataContext = Services.GetRequiredService<MainWindowViewModel>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    public void ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton(Configuration);

        // 2. Register DbContext
        // Replace YourDbContextName with the actual name (e.g., AppDbContext)
        // Replace "DefaultConnection" if you used a different name in appsettings.json
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        // 3. Register ViewModels and other services
        // Example:
        services.AddScoped<LoginViewModel>();
        services.AddScoped<HomeViewModel>();
        services.AddScoped<MainWindowViewModel>();
        // services.AddTransient<SomeOtherViewModel>();
        // services.AddSingleton<IMyService, MyServiceImplementation>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<LoginView>();
        services.AddSingleton<HomeView>();
        //Repositories
        services.AddScoped<IVehicleRepository, VehicleRepository>();

    }
    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}