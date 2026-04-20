using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using OPSW11.Models;
using OPSW11.Services;

namespace OPSW11.Views;

public partial class LogsView : UserControl
{
    private readonly LoggingService _logger;
    private string _filtr = "All";

    public LogsView()
    {
        InitializeComponent();

        _logger = LoggingService.Instance;

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
        LogCountText.Text = $"{LogListBox.Items.Count} wpisów";
    }

    private void OpenLogFile_Click(object sender, RoutedEventArgs e)
    {
        if (File.Exists(_logger.LogFilePath))
        {
            Process.Start(new ProcessStartInfo(_logger.LogFilePath) { UseShellExecute = true });
        }
        else
        {
            MessageBox.Show("Plik dziennika nie został jeszcze utworzony.", "Dziennik",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void ClearLogs_Click(object sender, RoutedEventArgs e)
    {
        var odpowiedz = MessageBox.Show(
            "Wyczyścić wszystkie wpisy z bieżącej sesji?",
            "Wyczyść dziennik",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (odpowiedz == MessageBoxResult.Yes)
        {
            _logger.Entries.Clear();
            AktualizujLicznik();
        }
    }
}
