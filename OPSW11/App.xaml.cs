using System.Windows;
using OPSW11.Helpers;

namespace OPSW11;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        if (!AdminHelper.IsRunningAsAdministrator())
        {
            AdminHelper.RestartWithElevation();
            return;
        }

        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
}