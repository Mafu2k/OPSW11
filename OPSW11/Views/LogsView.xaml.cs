using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using OPSW11.Localization;
using OPSW11.Models;
using OPSW11.Services;

namespace OPSW11.Views;

public partial class LogsView : UserControl
{
    private readonly LoggingService _logger = LoggingService.Instance;
    private string _filtr = "All";

    public LogsView()
    {
        InitializeComponent();

        LogListBox.ItemsSource = _logger.Entries;

        _logger.Entries.CollectionChanged += (_, _) => ZastosujFiltr();
        _logger.EntryAdded                += _ => ScrollDoDolu();

        AktualizujLicznik();
    }

    private void Filter_Changed(object sender, RoutedEventArgs e)
    {
        if      (FilterAll.IsChecked     == true) _filtr = "All";
        else if (FilterWarning.IsChecked == true) _filtr = "Warning";
        else if (FilterError.IsChecked   == true) _filtr = "Error";
        else if (FilterSuccess.IsChecked == true) _filtr = "Success";

        ZastosujFiltr();
    }

    private void ZastosujFiltr()
    {
        if (LogListBox == null) return;

        Dispatcher.Invoke(() =>
        {
            if (_filtr == "All")
            {
                LogListBox.ItemsSource = _logger.Entries;
                AktualizujLicznik();
                return;
            }

            if (!Enum.TryParse<LogLevel>(_filtr, out var poziom)) return;

            LogListBox.ItemsSource = new ObservableCollection<LogEntry>(
                _logger.Entries.Where(e => e.Level == poziom));

            AktualizujLicznik();
        });
    }

    private void ScrollDoDolu()
    {
        Dispatcher.Invoke(() =>
        {
            if (LogListBox.Items.Count > 0)
                LogListBox.ScrollIntoView(LogListBox.Items[^1]);
        });
    }

    private void AktualizujLicznik()
    {
        LogCountText.Text = Loc.F("Log_Count", LogListBox.Items.Count);
    }

    private void OpenLogFile_Click(object sender, RoutedEventArgs e)
    {
        if (File.Exists(_logger.LogFilePath))
        {
            Process.Start(new ProcessStartInfo(_logger.LogFilePath) { UseShellExecute = true });
        }
        else
        {
            MessageBox.Show(Loc.T("Log_NotCreated"), Loc.T("Log_DialogTitle"),
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void ExportLogs_Click(object sender, RoutedEventArgs e)
    {
        if (_logger.Entries.Count == 0)
        {
            MessageBox.Show(Loc.T("Log_ExportEmpty"), Loc.T("Log_ExportTitle"),
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var dialog = new SaveFileDialog
        {
            Title    = Loc.T("Log_ExportTitle"),
            Filter   = "Log files (*.log)|*.log|Text files (*.txt)|*.txt|All files (*.*)|*.*",
            FileName = $"OPSW11_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log"
        };

        if (dialog.ShowDialog() != true) return;

        try
        {
            var sb = new StringBuilder();
            foreach (var wpis in _logger.Entries)
                sb.AppendLine($"[{wpis.Timestamp:HH:mm:ss}] [{wpis.LevelTag,-7}] {wpis.Message}");

            File.WriteAllText(dialog.FileName, sb.ToString());

            MessageBox.Show(Loc.F("Log_ExportDone", dialog.FileName), Loc.T("Log_ExportTitle"),
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Export: {ex.Message}");
        }
    }

    private void ClearLogs_Click(object sender, RoutedEventArgs e)
    {
        var odpowiedz = MessageBox.Show(
            Loc.T("Log_ClearConfirm"),
            Loc.T("Log_ClearTitle"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (odpowiedz == MessageBoxResult.Yes)
        {
            _logger.Entries.Clear();
            AktualizujLicznik();
        }
    }
}
