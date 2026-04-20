using OPSW11.Helpers;
using OPSW11.Models;

namespace OPSW11.Services;

public class RepairService
{
    private readonly LoggingService _logger;

    public RepairService(LoggingService logger)
    {
        _logger = logger;
    }

    public async Task<OperationResult> RunSfcAsync(
        Action<string>? progressCallback = null,
        CancellationToken ct = default)
    {
        _logger.LogInfo("Uruchamianie SFC /scannow — może potrwać kilkanaście minut...");

        var wynik = await ProcessHelper.RunAsync(
            SystemPaths.Sfc, "/scannow",
            linia =>
            {
                progressCallback?.Invoke(linia);
                _logger.LogInfo(linia);
            },
            ct);

        if (wynik.IsSuccess) _logger.LogSuccess("SFC zakończony pomyślnie.");
        else                 _logger.LogError($"SFC zakończył się z problemami: {wynik.Message}");

        return wynik;
    }

    public async Task<OperationResult> RunDismAsync(
        Action<string>? progressCallback = null,
        CancellationToken ct = default)
    {
        _logger.LogInfo("Uruchamianie DISM /RestoreHealth — może potrwać do 45 minut...");

        var wynik = await ProcessHelper.RunAsync(
            SystemPaths.Dism, "/Online /Cleanup-Image /RestoreHealth",
            linia =>
            {
                progressCallback?.Invoke(linia);
                _logger.LogInfo(linia);
            },
            ct);

        if (wynik.IsSuccess) _logger.LogSuccess("DISM zakończony pomyślnie.");
        else                 _logger.LogError($"DISM zakończył się z problemami: {wynik.Message}");

        return wynik;
    }

    public async Task<OperationResult> ResetWindowsUpdateAsync(CancellationToken ct = default)
    {
        _logger.LogInfo("Resetowanie składników Windows Update...");

        string winDir       = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        string softwareDist = Path.Combine(winDir, "SoftwareDistribution");
        string catroot2     = Path.Combine(winDir, "System32", "catroot2");

        var kroki = new (string opis, string exe, string args)[]
        {
            ("Zatrzymywanie BITS",               SystemPaths.Net,      "stop bits /y"),
            ("Zatrzymywanie Windows Update",     SystemPaths.Net,      "stop wuauserv /y"),
            ("Zatrzymywanie AppID",              SystemPaths.Net,      "stop appidsvc /y"),
            ("Zatrzymywanie CryptSvc",           SystemPaths.Net,      "stop cryptsvc /y"),
            ("Czyszczenie SoftwareDistribution", SystemPaths.Cmd,      $"/c rmdir /s /q \"{softwareDist}\""),
            ("Czyszczenie catroot2",             SystemPaths.Cmd,      $"/c rmdir /s /q \"{catroot2}\""),
            ("Rejestracja komponentów WU",       SystemPaths.Regsvr32, "/s atl.dll"),
            ("Uruchamianie BITS",                SystemPaths.Net,      "start bits"),
            ("Uruchamianie Windows Update",      SystemPaths.Net,      "start wuauserv"),
            ("Uruchamianie AppID",               SystemPaths.Net,      "start appidsvc"),
            ("Uruchamianie CryptSvc",            SystemPaths.Net,      "start cryptsvc"),
        };

        foreach (var (opis, exe, args) in kroki)
        {
            ct.ThrowIfCancellationRequested();
            _logger.LogInfo(opis);
            var wynik = await ProcessHelper.RunAsync(exe, args, cancellationToken: ct);
            if (!wynik.IsSuccess)
                _logger.LogWarning($"  '{opis}' zakończył się z problemem — kontynuuję.");
        }

        _logger.LogSuccess("Składniki Windows Update zresetowane. Zalecany restart.");
        return OperationResult.Success("Reset Windows Update zakończony. Zalecany restart.");
    }
}
