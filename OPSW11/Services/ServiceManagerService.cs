using System.Diagnostics;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using OPSW11.Helpers;
using OPSW11.Models;

namespace OPSW11.Services;

public partial class ServiceManagerService
{
    private readonly LoggingService _logger;

    // Windows service names are restricted to a small character set. We enforce it
    // ourselves before the name is ever interpolated into an sc.exe argument, so a
    // malformed/hostile name can never inject additional command-line switches.
    [GeneratedRegex(@"^[A-Za-z0-9_.\-]{1,256}$")]
    private static partial Regex ServiceNameRegex();

    public static bool IsValidServiceName(string? name)
        => !string.IsNullOrWhiteSpace(name) && ServiceNameRegex().IsMatch(name);

    public static readonly (string Name, string DisplayName, string Reason)[] OptimizableServices =
    [
        ("SysMain",    "SysMain (Superfetch)",      "Pre-loads apps into RAM. Not beneficial on SSDs."),
        ("WSearch",    "Windows Search",            "File indexing. Disable to reduce I/O on low-spec systems."),
        ("DiagTrack",  "Connected User Experiences","Microsoft telemetry. Disable for privacy & CPU savings."),
        ("WerSvc",     "Windows Error Reporting",   "Reports crashes to Microsoft. Safe to disable."),
        ("RetailDemo", "Retail Demo Service",        "OEM demo mode service. Not needed on personal machines."),
        ("MapsBroker", "Downloaded Maps Manager",   "Background maps updates. Disable if offline maps unused."),
    ];

    public ServiceManagerService(LoggingService logger)
    {
        _logger = logger;
    }

    public async Task<OperationResult> SetServicesToManualAsync(IEnumerable<string> serviceNames)
    {
        return await Task.Run(() =>
        {
            int changed = 0;

            foreach (string name in serviceNames)
            {
                if (!IsValidServiceName(name))
                {
                    _logger.LogWarning($"  Skipped invalid service name: '{name}'");
                    continue;
                }

                try
                {
                    _logger.LogInfo($"Setting '{name}' startup to Manual...");

                    var psi = new ProcessStartInfo(SystemPaths.Sc, $"config \"{name}\" start= demand")
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };
                    using var proc = Process.Start(psi);
                    proc?.WaitForExit(5000);

                    using var sc = new ServiceController(name);
                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                    }

                    _logger.LogSuccess($"  '{name}' → Manual, stopped.");
                    changed++;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"  Failed to configure '{name}': {ex.Message}");
                }
            }

            return OperationResult.Success($"Configured {changed} service(s).");
        });
    }

    public async Task<Dictionary<string, ServiceControllerStatus?>> GetServiceStatusesAsync(string[] serviceNames)
    {
        return await Task.Run(() =>
        {
            var statuses = new Dictionary<string, ServiceControllerStatus?>();

            foreach (string name in serviceNames)
            {
                try
                {
                    using var sc = new ServiceController(name);
                    statuses[name] = sc.Status;
                }
                catch
                {
                    statuses[name] = null; // Service does not exist on this machine
                }
            }

            return statuses;
        });
    }
}
