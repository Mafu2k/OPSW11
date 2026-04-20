using System.IO;

namespace OPSW11.Helpers;

internal static class SystemPaths
{
    private static readonly string Sys32 = Environment.SystemDirectory;

    public static string Net        => Path.Combine(Sys32, "net.exe");
    public static string Sc         => Path.Combine(Sys32, "sc.exe");
    public static string Netsh      => Path.Combine(Sys32, "netsh.exe");
    public static string Ipconfig   => Path.Combine(Sys32, "ipconfig.exe");
    public static string Sfc        => Path.Combine(Sys32, "sfc.exe");
    public static string Dism       => Path.Combine(Sys32, "Dism.exe");
    public static string Defrag     => Path.Combine(Sys32, "defrag.exe");
    public static string Regsvr32   => Path.Combine(Sys32, "regsvr32.exe");
    public static string Cmd        => Path.Combine(Sys32, "cmd.exe");
    public static string Shutdown   => Path.Combine(Sys32, "shutdown.exe");
    public static string PowerShell => Path.Combine(Sys32, "WindowsPowerShell", "v1.0", "powershell.exe");
}
