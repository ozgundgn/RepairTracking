using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Input;
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
using Avalonia.Threading;
using QuestPDF.Infrastructure;
using RepairTracking.Services;
using RepairTracking.ViewModels.Factories;

namespace RepairTracking;

public class App : Application
{
    private InactivityService? _inactivityService;
    private IServiceProvider? Services { get; set; } // Property to hold the service provider
    private IConfiguration Configuration { get; set; } // Property to hold the configuration
    private INavigationService NavigationService { get; set; }

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
            NavigationService = new NavigationService(mainWindowViewModel, Services,
                Services.GetRequiredService<IViewModelFactory>());
            AppServices.NavigationService = NavigationService;

            // ========== AUTO-LOGOUT SETUP ==========

            // 1. Initialize the Inactivity Service with a timeout (e.g., 15 minutes)
            _inactivityService = new InactivityService(TimeSpan.FromMinutes(45));
            _inactivityService.OnInactive += HandleInactiveUser; // Subscribe to the event
            _inactivityService.Start(); // Start the timer

            // 2. Hook into the global input manager to detect activity
            Avalonia.Input.InputElement.PointerPressedEvent.AddClassHandler<Control>((_, _) =>
            {
                _inactivityService?.ResetTimer();
            });
            Avalonia.Input.InputElement.KeyDownEvent.AddClassHandler<Control>((_, _) =>
            {
                _inactivityService?.ResetTimer();
            });
            // ========================================
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void HandleInactiveUser()
    {
        Console.WriteLine("User is inactive. Logging out now.");

        // IMPORTANT: Ensure this code runs on the UI thread.
        // DispatcherTimer already ensures this, but it's good practice to be explicit.
        Dispatcher.UIThread.Post(() =>
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // This is where you put your actual logout logic.
                // For example, show the login window and close the main window.

                // 1. Clear any session data (current user, tokens, etc.)
                // e.g., SessionManager.ClearCurrentUser();

                // 2. Navigate back to the LoginView
                // How you do this depends on your navigation system.
                // A simple approach is to close the main window and open a new login window.
                // var currentMainWindow = desktop.MainWindow;

                // var loginWindow = new LoginView(); // Assuming you have a LoginWindow
                // loginWindow.Show();
                AppServices.UserSessionService.Logout();
                AppServices.NavigationService.NavigateToLogin();
                // currentMainWindow?.Close();
            }
        });
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

        services.AddTransient<LoginView>();
        services.AddTransient<HomeView>();

        //Repositories
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomersVehiclesRepository, CustomersVehiclesRepository>();
        services.AddScoped<IRenovationRepository, RenovationRepository>();
        //unitofwork
        services.AddSingleton<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IViewModelFactory, ViewModelFactory>();
        services.AddSingleton<IDialogService, DialogService>();
        QuestPDF.Settings.License = LicenseType.Community;
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