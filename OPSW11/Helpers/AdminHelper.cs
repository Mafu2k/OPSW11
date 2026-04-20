using System.ComponentModel;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows;

namespace OPSW11.Helpers;

public static class AdminHelper
{
    public static bool IsRunningAsAdministrator()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public static void RestartWithElevation()
    {
        var executablePath = Process.GetCurrentProcess().MainModule?.FileName;
        if (executablePath is null) return;

        var startInfo = new ProcessStartInfo
        {
            FileName       = executablePath,
            UseShellExecute = true,
            Verb           = "runas"
        };

        try
        {
            Process.Start(startInfo);
        }
        catch (Win32Exception)
        {
            // Użytkownik odrzucił monit UAC — powiadom i wyjdź
            MessageBox.Show(
                "Aplikacja wymaga uprawnień administratora.\nUruchom ponownie i zaakceptuj monit UAC.",
                "Wymagane uprawnienia administratora",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        Environment.Exit(0);
    }
}
