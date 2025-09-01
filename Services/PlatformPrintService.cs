using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RepairTracking.Services;

public static class PlatformPrintService
{
    public static async Task<bool> PrintFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            System.Console.WriteLine($"File not found: {filePath}");
            return false;
        }

        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // For Windows, use ShellExecute verb "print"
                var process = Process.Start(new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true,
                    Verb = "print"
                });
                // Note: ShellExecute "print" might not wait for the print job to finish
                // or even for the application to open. It just dispatches the command.
                return true;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // For macOS, use "open -t" to print. This usually opens the default
                // application with the print dialog.
                var process = Process.Start("open", $"-t \"{filePath}\"");
                await process.WaitForExitAsync();
               return process.ExitCode == 0;
                // var printProcess = new Process
                // {
                //     StartInfo = new ProcessStartInfo
                //     {
                //         FileName = "lpr",
                //         Arguments = $"-t \"{filePath}\"",
                //         RedirectStandardOutput = true,
                //         RedirectStandardError = true,
                //         UseShellExecute = false,
                //         CreateNoWindow = false
                //     }
                // };
                //
                // printProcess.Start();
                // return true;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // For Linux, use "lp" command (CUPS print service).
                // This sends directly to the default printer. For a dialog, it's more complex.
                var process = Process.Start("lp", $"\"{filePath}\"");
                await process.WaitForExitAsync();
                return process.ExitCode == 0;
            }
            else
            {
                System.Console.WriteLine("Unsupported operating system for direct printing.");
                return false;
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"Error printing file '{filePath}': {ex.Message}");
            return false;
        }
    }
}