using System.Windows;
using System.Windows.Controls;
using OPSW11.Helpers;
using OPSW11.Models;

namespace OPSW11.Views;

public partial class AdvancedFixView : UserControl
{
    private CancellationTokenSource? _cts;

    public AdvancedFixView()
    {
        InitializeComponent();
    }

    private async void RunButton_Click(object sender, RoutedEventArgs e)
    {
        var odpowiedz = MessageBox.Show(
            "Zaawansowana naprawa uruchomi SFC, DISM oraz zresetuje składniki Windows Update.\n\n" +
            "Operacja może potrwać 15–45 minut. Wymagany będzie restart komputera.\n\n" +
            "Przed wykonaniem zostanie utworzony punkt przywracania.\n\nKontynuować?",
            "Potwierdź naprawę zaawansowaną",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (odpowiedz != MessageBoxResult.Yes) return;

        _cts = new CancellationTokenSource();
        SetUiBusy(true);

        try
        {
            var ct = _cts.Token;

            CurrentOpText.Text = "Tworzenie punktu przywracania systemu…";
            await AppServices.Backup.CreateSystemRestorePointAsync("WO11 — przed zaawansowaną naprawą");

            await Krok("Uruchamianie SFC /scannow…",
                ct => AppServices.Repair.RunSfcAsync(DodajOutput, ct), ct);

            await Krok("Uruchamianie DISM /RestoreHealth…",
                ct => AppServices.Repair.RunDismAsync(DodajOutput, ct), ct);

            await Krok("Resetowanie składników Windows Update…",
                ct => AppServices.Repair.ResetWindowsUpdateAsync(ct), ct);

            SetUiBusy(false);
            RestartBanner.Visibility = Visibility.Visible;
        }
        catch (OperationCanceledException)
        {
            DodajOutput("\n✕ Anulowano przez użytkownika.");
            SetUiBusy(false);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        _cts?.Cancel();
    }

    private async Task Krok(string nazwa, Func<CancellationToken, Task<OperationResult>> operacja, CancellationToken ct)
    {
        CurrentOpText.Text = nazwa;
        DodajOutput($"\n═══ {nazwa} ═══");

        var wynik = await operacja(ct);
        DodajOutput(wynik.IsSuccess ? "✓ Zakończono." : $"⚠ {wynik.Message}");

        OverallProgress.Value++;
    }

    private void DodajOutput(string tekst)
    {
        Dispatcher.Invoke(() =>
        {
            LiveOutputText.Text += tekst + Environment.NewLine;
            OutputScroll.ScrollToEnd();
        });
    }

    private void SetUiBusy(bool zajety)
    {
        RunButton.IsEnabled             = !zajety;
        CancelButton.Visibility         = zajety ? Visibility.Visible : Visibility.Collapsed;
        ProgressPanel.Visibility        = Visibility.Visible;
        OverallProgress.IsIndeterminate = zajety;

        if (!zajety)
        {
            CurrentOpText.Text              = "Wszystkie operacje zakończone";
            OverallProgress.Value           = 4;
            OverallProgress.IsIndeterminate = false;
        }
    }

    private void RestartNowButton_Click(object sender, RoutedEventArgs e)
    {
        var odpowiedz = MessageBox.Show(
            "Komputer zostanie uruchomiony ponownie. Zapisz wszystkie otwarte pliki.\n\nUruchomić ponownie?",
            "Potwierdzenie restartu",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (odpowiedz == MessageBoxResult.Yes)
            System.Diagnostics.Process.Start(SystemPaths.Shutdown, "/r /t 5 /c \"WO11: restart po naprawie systemu.\"");
    }
}
