using OPSW11.Helpers;
using OPSW11.Models;

namespace OPSW11.Services;

public class NetworkService
{
    private readonly LoggingService _logger;

    public NetworkService(LoggingService logger)
    {
        _logger = logger;
    }

    public async Task<OperationResult> FlushDnsAsync(CancellationToken ct = default)
    {
        _logger.LogInfo("Reset cache DNS...");
        var wynik = await ProcessHelper.RunAsync(SystemPaths.Ipconfig, "/flushdns", cancellationToken: ct);

        if (wynik.IsSuccess) _logger.LogSuccess("Cache DNS wyczyszczony.");
        else                 _logger.LogError($"Błąd DNS flush: {wynik.Message}");

        return wynik;
    }

    public async Task<OperationResult> RestartNetworkServicesAsync(CancellationToken ct = default)
    {
        _logger.LogInfo("Restart usług sieciowych...");

        string[] uslugi = ["dhcp", "dnscache", "NlaSvc", "netprofm"];

        foreach (string usluga in uslugi)
        {
            ct.ThrowIfCancellationRequested();
            await ProcessHelper.RunAsync(SystemPaths.Net, $"stop \"{usluga}\" /y", cancellationToken: ct);
            await Task.Delay(300, ct);
            await ProcessHelper.RunAsync(SystemPaths.Net, $"start \"{usluga}\"", cancellationToken: ct);
            _logger.LogInfo($"Zrestartowano: {usluga}");
        }

        _logger.LogSuccess("Usługi sieciowe zrestartowane.");
        return OperationResult.Success("Usługi sieciowe zrestartowane.");
    }

    public async Task<OperationResult> ResetNetworkStackAsync(CancellationToken ct = default)
    {
        _logger.LogInfo("Pełny reset stosu sieciowego...");

        var komendy = new (string exe, string args)[]
        {
            (SystemPaths.Netsh,    "winsock reset"),
            (SystemPaths.Netsh,    "int ip reset"),
            (SystemPaths.Netsh,    "int ipv4 reset"),
            (SystemPaths.Netsh,    "int ipv6 reset"),
            (SystemPaths.Ipconfig, "/release"),
            (SystemPaths.Ipconfig, "/renew"),
            (SystemPaths.Ipconfig, "/flushdns"),
        };

        foreach (var (exe, args) in komendy)
        {
            ct.ThrowIfCancellationRequested();
            _logger.LogInfo($"  → {Path.GetFileNameWithoutExtension(exe)} {args}");
            var wynik = await ProcessHelper.RunAsync(exe, args, cancellationToken: ct);

            if (!wynik.IsSuccess)
                _logger.LogWarning($"  '{Path.GetFileNameWithoutExtension(exe)} {args}' zakończył się błędem — kontynuuję.");
        }

        _logger.LogSuccess("Reset stosu sieciowego zakończony. Zalecany restart.");
        return OperationResult.Success("Reset stosu sieciowego zakończony. Zalecany restart.");
    }
}
