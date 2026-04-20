using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using OPSW11.Models;

namespace OPSW11.Services;

public class LoggingService
{
    public static LoggingService Instance { get; } = new LoggingService();

    public ObservableCollection<LogEntry> Entries { get; } = new();

    public event Action<LogEntry>? EntryAdded;

    private readonly string _logFile;
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    private LoggingService()
    {
        string folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "WO11", "Logi");

        Directory.CreateDirectory(folder);
        _logFile = Path.Combine(folder, $"{DateTime.Now:yyyy-MM-dd}.log");

        UsunStareLogi(folder);
    }

    public void LogInfo(string msg)    => Dodaj(LogLevel.Info,    msg);
    public void LogWarning(string msg) => Dodaj(LogLevel.Warning, msg);
    public void LogError(string msg)   => Dodaj(LogLevel.Error,   msg);
    public void LogSuccess(string msg) => Dodaj(LogLevel.Success, msg);

    private void Dodaj(LogLevel poziom, string msg)
    {
        var wpis = new LogEntry { Level = poziom, Message = msg };

        System.Windows.Application.Current?.Dispatcher.Invoke(() => Entries.Add(wpis));
        EntryAdded?.Invoke(wpis);
        _ = ZapiszAsync(wpis);
    }

    private async Task ZapiszAsync(LogEntry wpis)
    {
        await _writeLock.WaitAsync();
        try
        {
            string linia = $"[{wpis.Timestamp:HH:mm:ss}] [{wpis.LevelTag,-7}] {wpis.Message}";
            await File.AppendAllTextAsync(_logFile, linia + Environment.NewLine);
        }
        catch { }
        finally
        {
            _writeLock.Release();
        }
    }

    private static void UsunStareLogi(string folder)
    {
        var cutoff = DateTime.Now.AddDays(-30);
        try
        {
            foreach (string file in Directory.GetFiles(folder, "*.log"))
            {
                if (File.GetLastWriteTime(file) < cutoff)
                    File.Delete(file);
            }
        }
        catch { }
    }

    public string LogFilePath => _logFile;
}
