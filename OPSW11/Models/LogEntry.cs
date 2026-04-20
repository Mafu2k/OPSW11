namespace OPSW11.Models;

public enum LogLevel { Info, Warning, Error, Success }

public class LogEntry
{
    public DateTime Timestamp { get; init; } = DateTime.Now;
    public LogLevel Level { get; init; }
    public string Message { get; init; } = string.Empty;

    public string FormattedTimestamp => Timestamp.ToString("HH:mm:ss");

    public string LevelTag => Level switch
    {
        LogLevel.Info    => "INFO",
        LogLevel.Warning => "WARN",
        LogLevel.Error   => "ERROR",
        LogLevel.Success => "OK",
        _                => "INFO"
    };
}
