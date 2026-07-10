using System.Windows;
using System.Windows.Controls;
using OPSW11.Helpers;
using OPSW11.Localization;
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
            Loc.T("AF_ConfirmMsg"),
            Loc.T("AF_ConfirmTitle"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (odpowiedz != MessageBoxResult.Yes) return;

        _cts = new CancellationTokenSource();
        SetUiBusy(true);

        try
        {
            var ct = _cts.Token;

            CurrentOpText.Text = Loc.T("AF_CreatingRestore");
            await AppServices.Backup.CreateSystemRestorePointAsync(Loc.T("AF_RestorePointDesc"));

            await Krok(Loc.T("AF_StepSfc"),
                ct => AppServices.Repair.RunSfcAsync(DodajOutput, ct), ct);

            await Krok(Loc.T("AF_StepDism"),
                ct => AppServices.Repair.RunDismAsync(DodajOutput, ct), ct);

            await Krok(Loc.T("AF_StepWu"),
                ct => AppServices.Repair.ResetWindowsUpdateAsync(ct), ct);

            SetUiBusy(false);
            RestartBanner.Visibility = Visibility.Visible;
        }
        catch (OperationCanceledException)
        {
            DodajOutput("\n" + Loc.T("AF_Cancelled"));
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
        DodajOutput(wynik.IsSuccess ? Loc.T("AF_StepDone") : $"⚠ {wynik.Message}");

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
            CurrentOpText.Text              = Loc.T("AF_AllDone");
            OverallProgress.Value           = 4;
            OverallProgress.IsIndeterminate = false;
        }
    }

    private void RestartNowButton_Click(object sender, RoutedEventArgs e)
    {
        var odpowiedz = MessageBox.Show(
            Loc.T("AF_RestartConfirmMsg"),
            Loc.T("AF_RestartConfirmTitle"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (odpowiedz == MessageBoxResult.Yes)
            System.Diagnostics.Process.Start(SystemPaths.Shutdown, $"/r /t 5 /c \"{Loc.T("AF_ShutdownComment")}\"");
    }
}
