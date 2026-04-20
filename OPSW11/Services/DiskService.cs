using OPSW11.Helpers;
using OPSW11.Models;

namespace OPSW11.Services;

public class DiskService
{
    private readonly LoggingService _logger;

    public DiskService(LoggingService logger)
    {
        _logger = logger;
    }

    public async Task<OperationResult> OptimizeDriveAsync(
        string driveLetter,
        Action<string>? progressCallback = null,
        CancellationToken ct = default)
    {
        if (driveLetter.Length != 1 || !char.IsAsciiLetter(driveLetter[0]))
            return OperationResult.Failure($"Nieprawidłowa litera dysku: '{driveLetter}'");

        bool isSsd = await DetectSolidStateDriveAsync(driveLetter, ct);

        if (isSsd)
        {
            _logger.LogInfo($"Dysk {driveLetter}: wykryto SSD — wykonywanie TRIM (retrim)...");
            var result = await ProcessHelper.RunAsync(
                SystemPaths.Defrag, $"{driveLetter}: /U /V /L",
                line => { progressCallback?.Invoke(line); _logger.LogInfo(line); },
                ct);

            if (result.IsSuccess) _logger.LogSuccess($"TRIM zakończony na {driveLetter}.");
            return result;
        }
        else
        {
            _logger.LogInfo($"Dysk {driveLetter}: wykryto HDD — wykonywanie defragmentacji...");
            var result = await ProcessHelper.RunAsync(
                SystemPaths.Defrag, $"{driveLetter}: /U /V",
                line => { progressCallback?.Invoke(line); _logger.LogInfo(line); },
                ct);

            if (result.IsSuccess) _logger.LogSuccess($"Defragmentacja zakończona na {driveLetter}.");
            return result;
        }
    }

    private async Task<bool> DetectSolidStateDriveAsync(string driveLetter, CancellationToken ct = default)
    {
        var result = await ProcessHelper.RunAsync(
            SystemPaths.PowerShell,
            $"-NoProfile -NonInteractive -Command \"try {{ (Get-Partition -DriveLetter {driveLetter} | Get-Disk | Get-PhysicalDisk).MediaType }} catch {{ 'Unknown' }}\"",
            cancellationToken: ct);

        return result.DetailedOutput?.Contains("SSD", StringComparison.OrdinalIgnoreCase) == true;
    }
}
