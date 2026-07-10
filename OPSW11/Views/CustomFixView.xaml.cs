using System.Windows;
using System.Windows.Controls;
using OPSW11.Helpers;
using OPSW11.Localization;
using OPSW11.Models;
using OPSW11.Services;

namespace OPSW11.Views;

public partial class CustomFixView : UserControl
{
    private readonly LoggingService _logger;
    private List<SelectableOperation> _operacje = [];
    private CancellationTokenSource? _cts;

    public CustomFixView()
    {
        InitializeComponent();
        _logger = LoggingService.Instance;
        ZbudujListeOperacji();
        OdswiezListy();
        AktualizujLicznik();

        LocalizationManager.Instance.LanguageChanged += OnLanguageChanged;
        Unloaded += (_, _) => LocalizationManager.Instance.LanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        var stany = _operacje.Select(o => o.IsSelected).ToList();
        ZbudujListeOperacji();
        for (int i = 0; i < _operacje.Count && i < stany.Count; i++)
            _operacje[i].IsSelected = stany[i];

        OdswiezListy();
        AktualizujLicznik();
    }

    private void ZbudujListeOperacji()
    {
        _operacje =
        [
            new SelectableOperation
            {
                Name        = Loc.T("Op_UserTemp_Name"),
                Description = Loc.T("Op_UserTemp_Desc"),
                GroupName   = "Cleanup",
                IsSafe      = true,
                Execute     = _ => AppServices.Cleanup.CleanUserTempFilesAsync()
            },
            new SelectableOperation
            {
                Name        = Loc.T("Op_WinTemp_Name"),
                Description = Loc.T("Op_WinTemp_Desc"),
                GroupName   = "Cleanup",
                IsSafe      = true,
                Execute     = _ => AppServices.Cleanup.CleanWindowsTempFilesAsync()
            },
            new SelectableOperation
            {
                Name        = Loc.T("Op_Prefetch_Name"),
                Description = Loc.T("Op_Prefetch_Desc"),
                GroupName   = "Cleanup",
                IsSafe      = true,
                Execute     = _ => AppServices.Cleanup.CleanPrefetchAsync()
            },
            new SelectableOperation
            {
                Name        = Loc.T("Op_RecycleBin_Name"),
                Description = Loc.T("Op_RecycleBin_Desc"),
                GroupName   = "Cleanup",
                IsSafe      = true,
                Execute     = _ => AppServices.Cleanup.CleanRecycleBinAsync()
            },
            new SelectableOperation
            {
                Name        = Loc.T("Op_WuCache_Name"),
                Description = Loc.T("Op_WuCache_Desc"),
                GroupName   = "Cleanup",
                IsSafe      = true,
                Execute     = _ => AppServices.Cleanup.CleanWindowsUpdateCacheAsync()
            },
            new SelectableOperation
            {
                Name        = Loc.T("Op_Dns_Name"),
                Description = Loc.T("Op_Dns_Desc"),
                GroupName   = "Network",
                IsSafe      = true,
                Execute     = ct => AppServices.Network.FlushDnsAsync(ct)
            },
            new SelectableOperation
            {
                Name        = Loc.T("Op_NetRestart_Name"),
                Description = Loc.T("Op_NetRestart_Desc"),
                GroupName   = "Network",
                IsSafe      = true,
                Execute     = ct => AppServices.Network.RestartNetworkServicesAsync(ct)
            },
            new SelectableOperation
            {
                Name        = Loc.T("Op_NetReset_Name"),
                Description = Loc.T("Op_NetReset_Desc"),
                GroupName   = "Network",
                IsSafe      = false,
                IsSelected  = false,
                Execute     = ct => AppServices.Network.ResetNetworkStackAsync(ct)
            },
            new SelectableOperation
            {
                Name        = Loc.T("Op_Sfc_Name"),
                Description = Loc.T("Op_Sfc_Desc"),
                GroupName   = "Repair",
                IsSafe      = true,
                IsSelected  = false,
                Execute     = ct => AppServices.Repair.RunSfcAsync(ct: ct)
            },
            new SelectableOperation
            {
                Name        = Loc.T("Op_Dism_Name"),
                Description = Loc.T("Op_Dism_Desc"),
                GroupName   = "Repair",
                IsSafe      = true,
                IsSelected  = false,
                Execute     = ct => AppServices.Repair.RunDismAsync(ct: ct)
            },
            new SelectableOperation
            {
                Name        = Loc.T("Op_WuReset_Name"),
                Description = Loc.T("Op_WuReset_Desc"),
                GroupName   = "Repair",
                IsSafe      = false,
                IsSelected  = false,
                Execute     = ct => AppServices.Repair.ResetWindowsUpdateAsync(ct)
            },
            new SelectableOperation
            {
                Name        = Loc.T("Op_SysMain_Name"),
                Description = Loc.T("Op_SysMain_Desc"),
                GroupName   = "Services",
                IsSafe      = false,
                IsSelected  = false,
                Execute     = _ => AppServices.ServiceManager.SetServicesToManualAsync(["SysMain"])
            },
            new SelectableOperation
            {
                Name        = Loc.T("Op_Search_Name"),
                Description = Loc.T("Op_Search_Desc"),
                GroupName   = "Services",
                IsSafe      = false,
                IsSelected  = false,
                Execute     = _ => AppServices.ServiceManager.SetServicesToManualAsync(["WSearch"])
            },
            new SelectableOperation
            {
                Name        = Loc.T("Op_Telemetry_Name"),
                Description = Loc.T("Op_Telemetry_Desc"),
                GroupName   = "Services",
                IsSafe      = false,
                IsSelected  = false,
                Execute     = _ => AppServices.ServiceManager.SetServicesToManualAsync(["DiagTrack"])
            },
            new SelectableOperation
            {
                Name        = Loc.T("Op_OptimizeDisk_Name"),
                Description = Loc.T("Op_OptimizeDisk_Desc"),
                GroupName   = "Disk",
                IsSafe      = true,
                IsSelected  = false,
                Execute     = ct => AppServices.Disk.OptimizeDriveAsync("C", ct: ct)
            },
        ];

        foreach (var op in _operacje)
            op.PropertyChanged += (_, _) => AktualizujLicznik();
    }

    // aktualizuje ItemsSource we wszystkich panelach
    private void OdswiezListy()
    {
        bool tylkoBezpieczne = SafeModeCheckBox.IsChecked == true;

        // filtrujemy jeśli tryb bezpieczny jest włączony
        var widoczne = tylkoBezpieczne
            ? _operacje.Where(o => o.IsSafe).ToList()
            : _operacje;

        CleanupOpsPanel.ItemsSource  = widoczne.Where(o => o.GroupName == "Cleanup").ToList();
        NetworkOpsPanel.ItemsSource  = widoczne.Where(o => o.GroupName == "Network").ToList();
        RepairOpsPanel.ItemsSource   = widoczne.Where(o => o.GroupName == "Repair").ToList();
        ServicesOpsPanel.ItemsSource = widoczne.Where(o => o.GroupName == "Services").ToList();
        DiskOpsPanel.ItemsSource     = widoczne.Where(o => o.GroupName == "Disk").ToList();
    }

    private void AktualizujLicznik()
    {
        int ile = _operacje.Count(o => o.IsSelected);
        SelectionCountText.Text     = Loc.F("CF_Count", ile);
        RunSelectedButton.IsEnabled = ile > 0;
    }

    private void SafeMode_Changed(object sender, RoutedEventArgs e)
    {
        // przy zmianie trybu bezpiecznego odświeżamy co jest widoczne
        OdswiezListy();
        AktualizujLicznik();
    }

    private void SelectAll_Click(object sender, RoutedEventArgs e)
    {
        bool tylkoBezpieczne = SafeModeCheckBox.IsChecked == true;

        foreach (var op in _operacje.Where(o => !tylkoBezpieczne || o.IsSafe))
            op.IsSelected = true;

        OdswiezListy();
        AktualizujLicznik();
    }

    private void RunSelected_Click(object sender, RoutedEventArgs e)
        => _ = RunSelectedAsync();

    private void CancelButton_Click(object sender, RoutedEventArgs e)
        => _cts?.Cancel();

    private async Task RunSelectedAsync()
    {
        var wybrane = _operacje.Where(o => o.IsSelected && o.Execute != null).ToList();

        bool saNiebezpieczne = wybrane.Any(o => !o.IsSafe);
        string notatka = saNiebezpieczne ? Loc.T("CF_DangerNote") : string.Empty;

        var odpowiedz = MessageBox.Show(
            Loc.F("CF_ConfirmMsg", wybrane.Count, notatka),
            Loc.T("CF_ConfirmTitle"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (odpowiedz != MessageBoxResult.Yes) return;

        _cts = new CancellationTokenSource();
        var ct = _cts.Token;
        SetUiBusy(true);

        await AppServices.Backup.CreateSystemRestorePointAsync(Loc.T("CF_RestorePointDesc"));

        int done = 0;
        foreach (var op in wybrane)
        {
            if (ct.IsCancellationRequested) break;

            CurrentOpText.Text = Loc.F("CF_Running", op.Name);
            ProgressBar.Value  = (double)done / wybrane.Count;

            try
            {
                var wynik = await op.Execute!(ct);
                ResultText.Text += wynik.IsSuccess
                    ? $"✓ {op.Name}\n"
                    : $"✗ {op.Name} — {wynik.Message}\n";
            }
            catch (OperationCanceledException)
            {
                ResultText.Text += $"✕ {op.Name} — {Loc.T("CF_Cancelled")}\n";
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{op.Name}: {ex.Message}");
                ResultText.Text += $"✗ {op.Name} — {Loc.T("CF_ErrorPrefix")}: {ex.Message}\n";
            }

            done++;
        }

        SetUiBusy(false);
    }

    private void SetUiBusy(bool zajety)
    {
        RunSelectedButton.IsEnabled = !zajety;
        CancelButton.Visibility     = zajety ? Visibility.Visible : Visibility.Collapsed;
        ProgressPanel.Visibility    = Visibility.Visible;
        ProgressBar.IsIndeterminate = zajety;

        if (!zajety)
        {
            CurrentOpText.Text          = Loc.T("CF_Done");
            ProgressBar.IsIndeterminate = false;
            ProgressBar.Value           = 1;
            AktualizujLicznik();
        }
    }
}
