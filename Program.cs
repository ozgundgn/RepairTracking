using Avalonia;
using System;
using System.IO;
using Avalonia.ReactiveUI;
using RepairTracking.Data;
using RepairTracking.ViewModels;
using RepairTracking.Views;
using Serilog;

namespace RepairTracking;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        string logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RepairTracking",
            "app.log"
        );
        var aa = Directory.Exists(Path.GetDirectoryName(logPath));
        if (!Directory.Exists(Path.GetDirectoryName(logPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(logPath) ?? string.Empty);
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
            .CreateLogger();
        try
        {
            Log.Information("Starting application");

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Log.Fatal(e.ExceptionObject as Exception, "Unhandled exception");
            };

            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Log.Fatal(e.Exception, "Unobserved task exception");
                e.SetObserved();
            };
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Application terminated unexpectedly");
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .UseReactiveUI()
            .LogToTrace();
}