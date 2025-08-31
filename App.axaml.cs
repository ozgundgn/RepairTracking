using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF;
using QuestPDF.Infrastructure;
using ReactiveUI;
using RepairTracking.Data;
using RepairTracking.Data.Models;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Repositories.Concrete;
using RepairTracking.Services;
using RepairTracking.ViewModels;
using RepairTracking.ViewModels.Factories;
using RepairTracking.Views;

namespace RepairTracking;

public class App : Application
{
    private InactivityService? _inactivityService;
    private IServiceProvider? Services { get; set; }
    private IConfiguration Configuration { get; set; }
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

        using (var scope = Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate(); // Creates & migrates
            if (!context.Users.Any()) // Example table
            {
                context.Users.AddRange(
                    new User
                    {
                        Name = "admin", Email = "admin@example.com", Password = "1234", UserName = "admin",
                        Surname = "admin", Passive = false, UserId = Guid.NewGuid()
                    }
                );
            }

            if (!context.Mails.Any())
            {
                context.Mails.AddRange(
                    new Mail
                    {
                        Subject = "Aracanız Hazır",
                        Type = "TESLIMAT",
                        Template =
                            "<p>&nbsp;&nbsp;Merhaba {MUSTERIADI},</p><br><p>&nbsp;Aracınızın tamir işlemi tamamlanmıştır. Rapor ekte yer almaktadır.</p><br><br><p> İyi günler dileriz.</p></p>",
                    }
                );
            }

            context.SaveChanges();
        }
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
            NavigationService = new NavigationService(mainWindowViewModel,
                Services.GetRequiredService<IViewModelFactory>());
            AppServices.NavigationService = NavigationService;

            // ========== AUTO-LOGOUT SETUP ==========

            // 1. Initialize the Inactivity Service with a timeout (e.g., 15 minutes)
            _inactivityService = new InactivityService(TimeSpan.FromMinutes(45));
            _inactivityService.OnInactive += HandleInactiveUser; // Subscribe to the event
            _inactivityService.Start(); // Start the timer

            // 2. Hook into the global input manager to detect activity
            InputElement.PointerPressedEvent.AddClassHandler<Control>((_, _) => { _inactivityService?.ResetTimer(); });
            InputElement.KeyDownEvent.AddClassHandler<Control>((_, _) => { _inactivityService?.ResetTimer(); });
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
                AppServices.UserSessionService.Logout();
                AppServices.NavigationService.NavigateToLogin();
            }
        });
    }

    private void ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton(Configuration);
        string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RepairTracking",
            "app.db"
        );
        if (!Directory.Exists(Path.GetDirectoryName(dbPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        // 3. Register ViewModels and other services
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
        services.AddScoped<IMailRepository, MailRepository>();
        //unitofwork
        services.AddSingleton<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IViewModelFactory, ViewModelFactory>();
        services.AddTransient<IDialogService, DialogService>();

        Settings.License = LicenseType.Community;
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}