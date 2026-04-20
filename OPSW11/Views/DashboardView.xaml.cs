using System.Windows.Controls;
using System.Windows.Threading;
using OPSW11.Helpers;
using OPSW11.Services;

namespace OPSW11.Views;

public partial class DashboardView : UserControl
{
    private readonly SystemInfoService _systemInfo;
    private readonly DispatcherTimer _refreshTimer;
    private bool _systemInfoLoaded;

    public DashboardView()
    {
        InitializeComponent();

        _systemInfo   = new SystemInfoService();
        _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
        _refreshTimer.Tick += async (_, _) =>
        {
            try { await RefreshMetricsAsync(); }
            catch { }
        };

        Loaded   += async (_, _) => { _refreshTimer.Start(); await RefreshMetricsAsync(); };
        Unloaded += (_, _) => { _refreshTimer.Stop(); _systemInfo.Dispose(); };
    }

    private async Task RefreshMetricsAsync()
    {
        var snapshot = await _systemInfo.GetSnapshotAsync();

        CpuValueText.Text     = $"{snapshot.CpuUsagePercent:F0}%";
        CpuProgressBar.Value  = snapshot.CpuUsagePercent;
        CpuDetailText.Text    = "Obciążenie procesora";

        RamValueText.Text     = $"{snapshot.RamUsedPercent:F0}%";
        RamProgressBar.Value  = snapshot.RamUsedPercent;
        RamDetailText.Text    = $"{FormatHelper.BytesToReadable(snapshot.RamUsedBytes)} / {FormatHelper.BytesToReadable(snapshot.RamTotalBytes)}";

        DiskValueText.Text    = $"{snapshot.DiskUsedPercent:F0}%";
        DiskProgressBar.Value = snapshot.DiskUsedPercent;
        DiskDetailText.Text   = $"{FormatHelper.BytesToReadable(snapshot.DiskUsedBytes)} / {FormatHelper.BytesToReadable(snapshot.DiskTotalBytes)}";

        UptimeText.Text       = FormatHelper.UptimeToReadable(snapshot.Uptime);

        if (!_systemInfoLoaded)
        {
            MachineNameText.Text    = snapshot.MachineName;
            OsVersionText.Text      = snapshot.OsVersion;
            ProcessorText.Text      = snapshot.ProcessorName;
            SystemSubtitleText.Text = $"{snapshot.MachineName} · {snapshot.OsVersion}";
            _systemInfoLoaded = true;
        }
    }
}
