namespace OPSW11.Models;

public class SystemSnapshot
{
    public double CpuUsagePercent { get; init; }
    public double RamUsedPercent { get; init; }
    public long RamUsedBytes { get; init; }
    public long RamTotalBytes { get; init; }
    public double DiskUsedPercent { get; init; }
    public long DiskUsedBytes { get; init; }
    public long DiskTotalBytes { get; init; }
    public TimeSpan Uptime { get; init; }
    public string OsVersion { get; init; } = string.Empty;
    public string MachineName { get; init; } = string.Empty;
    public string ProcessorName { get; init; } = string.Empty;
}
