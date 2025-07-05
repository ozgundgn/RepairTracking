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
using ReactiveUI;
using RepairTracking.Data;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Repositories.Concrete;
using RepairTracking.ViewModels;
using RepairTracking.Views;
using Avalonia.ReactiveUI;
using RepairTracking.Services;

namespace RepairTracking;

public class App : Application
{
    public IServiceProvider? Services { get; private set; } // Property to hold the service provider
    public IConfiguration Configuration { get; private set; } // Property to hold the configuration
    public INavigationService NavigationService { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
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
            var mainWindowViewModel = Services.GetRequiredService<MainWindowViewModel>();
            mainWindowViewModel.CurrentView = Services.GetRequiredService<LoginViewModel>();
            desktop.MainWindow = new MainWindow()
            {
                DataContext = mainWindowViewModel
            };
            NavigationService = new NavigationService(mainWindowViewModel,Services);
            AppServices.NavigationService = NavigationService;
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
        services.AddTransient<LoginViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<AddCustomerViewModel>();
        services.AddTransient<UserProfileHeaderViewModel>();
        services.AddTransient<VehicleDetailsViewModel>();
        // services.AddTransient<SomeOtherViewModel>();
        // services.AddSingleton<IMyService, MyServiceImplementation>();

        services.AddTransient<LoginView>();
        services.AddTransient<HomeView>();

        //Repositories
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomersVehiclesRepository, CustomersVehiclesRepository>();
        services.AddScoped<IRenovationRepository, RenovationRepository>();
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