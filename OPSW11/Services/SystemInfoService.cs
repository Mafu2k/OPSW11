using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using OPSW11.Models;

namespace OPSW11.Services;

public class SystemInfoService : IDisposable
{
    private readonly PerformanceCounter _cpuCounter;
    private bool _disposed;

    public SystemInfoService()
    {
        _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        _cpuCounter.NextValue();
    }

    public async Task<SystemSnapshot> GetSnapshotAsync()
    {
        await Task.Delay(500);

        double cpuUsage = Math.Round(_cpuCounter.NextValue(), 1);

        var (ramTotal, ramAvailable) = await Task.Run(GetMemoryInfo);
        long ramUsed = ramTotal - ramAvailable;

        TimeSpan uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);

        var sysDrive = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory) ?? "C:\\");
        long diskTotal = sysDrive.TotalSize;
        long diskFree  = sysDrive.AvailableFreeSpace;
        long diskUsed  = diskTotal - diskFree;

        string processorName = await Task.Run(GetProcessorName);

        return new SystemSnapshot
        {
            CpuUsagePercent  = cpuUsage,
            RamUsedBytes     = ramUsed,
            RamTotalBytes    = ramTotal,
            RamUsedPercent   = ramTotal > 0 ? Math.Round((double)ramUsed / ramTotal * 100, 1) : 0,
            DiskUsedBytes    = diskUsed,
            DiskTotalBytes   = diskTotal,
            DiskUsedPercent  = diskTotal > 0 ? Math.Round((double)diskUsed / diskTotal * 100, 1) : 0,
            Uptime           = uptime,
            OsVersion        = $"Windows {Environment.OSVersion.Version.Major} (Build {Environment.OSVersion.Version.Build})",
            MachineName      = Environment.MachineName,
            ProcessorName    = processorName
        };
    }

    private static (long Total, long Available) GetMemoryInfo()
    {
        var status = new MemoryStatusEx { dwLength = (uint)Marshal.SizeOf<MemoryStatusEx>() };

        if (!GlobalMemoryStatusEx(ref status))
            return (0, 0);

        return ((long)status.ullTotalPhys, (long)status.ullAvailPhys);
    }

    private static string GetProcessorName()
    {
        return Registry.GetValue(
            @"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0",
            "ProcessorNameString",
            "Unknown CPU")?.ToString()?.Trim() ?? "Unknown CPU";
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MemoryStatusEx
    {
        public uint   dwLength;
        public uint   dwMemoryLoad;
        public ulong  ullTotalPhys;
        public ulong  ullAvailPhys;
        public ulong  ullTotalPageFile;
        public ulong  ullAvailPageFile;
        public ulong  ullTotalVirtual;
        public ulong  ullAvailVirtual;
        public ulong  ullAvailExtendedVirtual;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx(ref MemoryStatusEx lpBuffer);

    public void Dispose()
    {
        if (_disposed) return;
        _cpuCounter.Dispose();
        _disposed = true;
    }
}
