using System.Windows;
using System.Windows.Controls;
using OPSW11.Helpers;
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
    }

    private void ZbudujListeOperacji()
    {
        _operacje =
        [
            new SelectableOperation
            {
                Name        = "Wyczyść %TEMP% (pliki użytkownika)",
                Description = "Usuwa zawartość %TEMP%. Pliki w użyciu są pomijane.",
                GroupName   = "Cleanup",
                IsSafe      = true,
                Execute     = _ => AppServices.Cleanup.CleanUserTempFilesAsync()
            },
            new SelectableOperation
            {
                Name        = "Wyczyść C:\\Windows\\Temp",
                Description = "Usuwa tymczasowe pliki systemowe. Zablokowane pomijane.",
                GroupName   = "Cleanup",
                IsSafe      = true,
                Execute     = _ => AppServices.Cleanup.CleanWindowsTempFilesAsync()
            },
            new SelectableOperation
            {
                Name        = "Wyczyść Prefetch",
                Description = "Usuwa pliki .pf — zostaną odtworzone przy następnym użyciu.",
                GroupName   = "Cleanup",
                IsSafe      = true,
                Execute     = _ => AppServices.Cleanup.CleanPrefetchAsync()
            },
            new SelectableOperation
            {
                Name        = "Opróżnij Kosz",
                Description = "Trwale usuwa wszystkie elementy z Kosza.",
                GroupName   = "Cleanup",
                IsSafe      = true,
                Execute     = _ => AppServices.Cleanup.CleanRecycleBinAsync()
            },
            new SelectableOperation
            {
                Name        = "Cache Windows Update",
                Description = "Zatrzymuje usługi WU i usuwa pobrane pliki aktualizacji.",
                GroupName   = "Cleanup",
                IsSafe      = true,
                Execute     = _ => AppServices.Cleanup.CleanWindowsUpdateCacheAsync()
            },
            new SelectableOperation
            {
                Name        = "Reset cache DNS",
                Description = "Uruchamia ipconfig /flushdns. Naprawia błędne rozwiązania DNS.",
                GroupName   = "Network",
                IsSafe      = true,
                Execute     = ct => AppServices.Network.FlushDnsAsync(ct)
            },
            new SelectableOperation
            {
                Name        = "Restart usług sieciowych",
                Description = "Restartuje DHCP, DNS Client, NLA i Network Profile Manager.",
                GroupName   = "Network",
                IsSafe      = true,
                Execute     = ct => AppServices.Network.RestartNetworkServicesAsync(ct)
            },
            new SelectableOperation
            {
                Name        = "Pełny reset stosu sieciowego (netsh)",
                Description = "Resetuje Winsock i TCP/IP. Wymagany restart.",
                GroupName   = "Network",
                IsSafe      = false,
                IsSelected  = false,
                Execute     = ct => AppServices.Network.ResetNetworkStackAsync(ct)
            },
            new SelectableOperation
            {
                Name        = "SFC /scannow — Sprawdzenie plików systemowych",
                Description = "Skanuje i naprawia uszkodzone pliki Windows. Trwa 10–30 minut.",
                GroupName   = "Repair",
                IsSafe      = true,
                IsSelected  = false,
                Execute     = ct => AppServices.Repair.RunSfcAsync(ct: ct)
            },
            new SelectableOperation
            {
                Name        = "DISM /RestoreHealth",
                Description = "Naprawia magazyn składników Windows. 15–45 minut.",
                GroupName   = "Repair",
                IsSafe      = true,
                IsSelected  = false,
                Execute     = ct => AppServices.Repair.RunDismAsync(ct: ct)
            },
            new SelectableOperation
            {
                Name        = "Reset składników Windows Update",
                Description = "Zatrzymuje WU, czyści SoftwareDistribution i catroot2.",
                GroupName   = "Repair",
                IsSafe      = false,
                IsSelected  = false,
                Execute     = ct => AppServices.Repair.ResetWindowsUpdateAsync(ct)
            },
            new SelectableOperation
            {
                Name        = "Wyłącz SysMain (Superfetch)",
                Description = "Ustawia SysMain na Ręczny. Zmniejsza zużycie RAM — zalecane na SSD.",
                GroupName   = "Services",
                IsSafe      = false,
                IsSelected  = false,
                Execute     = _ => AppServices.ServiceManager.SetServicesToManualAsync(["SysMain"])
            },
            new SelectableOperation
            {
                Name        = "Wyłącz indeksowanie Windows Search",
                Description = "Ustawia WSearch na Ręczny. Zmniejsza I/O.",
                GroupName   = "Services",
                IsSafe      = false,
                IsSelected  = false,
                Execute     = _ => AppServices.ServiceManager.SetServicesToManualAsync(["WSearch"])
            },
            new SelectableOperation
            {
                Name        = "Wyłącz telemetrię (DiagTrack)",
                Description = "Ustawia DiagTrack na Ręczny. Prywatność i oszczędność CPU.",
                GroupName   = "Services",
                IsSafe      = false,
                IsSelected  = false,
                Execute     = _ => AppServices.ServiceManager.SetServicesToManualAsync(["DiagTrack"])
            },
            new SelectableOperation
            {
                Name        = "Optymalizuj dysk systemowy (C:)",
                Description = "Wykrywa SSD/HDD i uruchamia TRIM (SSD) lub defragmentację (HDD).",
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
        SelectionCountText.Text     = $"{ile} operacji wybranych";
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
        string notatka = saNiebezpieczne
            ? "\n\n⚠ Niektóre wybrane operacje mogą być destrukcyjne."
            : string.Empty;

        var odpowiedz = MessageBox.Show(
            $"Zostanie uruchomionych {wybrane.Count} operacji.{notatka}\n\nKontynuować?",
            "Potwierdź naprawę własną",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (odpowiedz != MessageBoxResult.Yes) return;

        _cts = new CancellationTokenSource();
        var ct = _cts.Token;
        SetUiBusy(true);

        await AppServices.Backup.CreateSystemRestorePointAsync("WO11 — przed naprawą własną");

        int done = 0;
        foreach (var op in wybrane)
        {
            if (ct.IsCancellationRequested) break;

            CurrentOpText.Text = $"Uruchamianie: {op.Name}";
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
                ResultText.Text += $"✕ {op.Name} — anulowano\n";
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Błąd w '{op.Name}': {ex.Message}");
                ResultText.Text += $"✗ {op.Name} — błąd: {ex.Message}\n";
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
            CurrentOpText.Text          = "Zakończono";
            ProgressBar.IsIndeterminate = false;
            ProgressBar.Value           = 1;
            AktualizujLicznik();
        }
    }
}
