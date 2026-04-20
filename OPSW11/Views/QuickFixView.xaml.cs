using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using OPSW11.Helpers;
using OPSW11.Models;

namespace OPSW11.Views;

public partial class QuickFixView : UserControl
{
    private readonly ObservableCollection<string> _statusItems = new();
    private CancellationTokenSource? _cts;
    private bool _wszystkoSzlo = true;

    public QuickFixView()
    {
        InitializeComponent();
        OperationStatusList.ItemsSource = _statusItems;
    }

    private async void RunButton_Click(object sender, RoutedEventArgs e)
    {
        var odpowiedz = MessageBox.Show(
            "Szybka naprawa wyczyści pliki tymczasowe, Prefetch, opróżni cache DNS i wyczyści pamięć podręczną Windows Update.\n\nKontynuować?",
            "Potwierdź szybką naprawę",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (odpowiedz != MessageBoxResult.Yes) return;

        _statusItems.Clear();
        _wszystkoSzlo = true;
        _cts = new CancellationTokenSource();
        SetUiBusy(true);

        try
        {
            var ct = _cts.Token;
            await Krok("Czyszczenie plików tymczasowych użytkownika...", ct => AppServices.Cleanup.CleanUserTempFilesAsync(), ct);
            await Krok("Czyszczenie Windows\\Temp...",                    ct => AppServices.Cleanup.CleanWindowsTempFilesAsync(), ct);
            await Krok("Czyszczenie Prefetch...",                          ct => AppServices.Cleanup.CleanPrefetchAsync(), ct);
            await Krok("Reset cache DNS...",                               ct => AppServices.Network.FlushDnsAsync(ct), ct);
            await Krok("Czyszczenie cache Windows Update...",             ct => AppServices.Cleanup.CleanWindowsUpdateCacheAsync(), ct);
        }
        catch (OperationCanceledException)
        {
            CurrentOperationText.Text = "Anulowano przez użytkownika.";
            _statusItems.Add("✕ Operacja anulowana.");
        }

        SetUiBusy(false);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        _cts?.Cancel();
    }

    private async Task Krok(string nazwa, Func<CancellationToken, Task<OperationResult>> operacja, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        CurrentOperationText.Text = nazwa;
        _statusItems.Add($"▶  {nazwa}");

        var wynik = await operacja(ct);

        if (wynik.IsSuccess)
            _statusItems.Add($"   ✓ {wynik.Message}");
        else
        {
            _statusItems.Add($"   ✗ {wynik.Message}");
            _wszystkoSzlo = false;
        }

        OverallProgress.Value++;
    }

    private void SetUiBusy(bool zajety)
    {
        RunButton.IsEnabled             = !zajety;
        CancelButton.Visibility         = zajety ? Visibility.Visible : Visibility.Collapsed;
        ProgressPanel.Visibility        = Visibility.Visible;
        OverallProgress.IsIndeterminate = zajety;
        OverallProgress.Value           = zajety ? 0 : 5;

        if (!zajety)
        {
            OverallProgress.IsIndeterminate = false;
            CurrentOperationText.Text = _wszystkoSzlo
                ? "Szybka naprawa zakończona pomyślnie."
                : "Zakończono z ostrzeżeniami — sprawdź wyniki powyżej.";
        }
    }
}
