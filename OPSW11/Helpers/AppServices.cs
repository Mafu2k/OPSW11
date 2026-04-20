using OPSW11.Services;

namespace OPSW11.Helpers;

internal static class AppServices
{
    private static readonly LoggingService Logger = LoggingService.Instance;

    public static CleanupService        Cleanup        { get; } = new(Logger);
    public static NetworkService        Network        { get; } = new(Logger);
    public static RepairService         Repair         { get; } = new(Logger);
    public static ServiceManagerService ServiceManager { get; } = new(Logger);
    public static DiskService           Disk           { get; } = new(Logger);
    public static BackupService         Backup         { get; } = new(Logger);
}
