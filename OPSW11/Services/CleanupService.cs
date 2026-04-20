using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using OPSW11.Helpers;
using OPSW11.Models;

namespace OPSW11.Services;

public class CleanupService
{
    private readonly LoggingService _logger;

    public CleanupService(LoggingService logger)
    {
        _logger = logger;
    }

    public async Task<OperationResult> CleanUserTempFilesAsync()
        => await CzyscFolderAsync(Path.GetTempPath(), "Temp użytkownika (%TEMP%)");

    public async Task<OperationResult> CleanWindowsTempFilesAsync()
        => await CzyscFolderAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"), "Windows Temp");

    public async Task<OperationResult> CleanPrefetchAsync()
        => await CzyscFolderAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch"), "Prefetch");

    public async Task<OperationResult> CleanWindowsUpdateCacheAsync()
    {
        _logger.LogInfo("Zatrzymywanie usług Windows Update...");

        return await Task.Run(() =>
        {
            try
            {
                RunService("stop", "wuauserv");
                RunService("stop", "bits");

                string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                string sciezka = Path.Combine(winDir, "SoftwareDistribution", "Download");
                if (Directory.Exists(sciezka))
                {
                    int usunieto = UsunZawartoscFolderu(sciezka);
                    _logger.LogSuccess($"Cache Windows Update wyczyszczony: {usunieto} elementów.");
                }

                RunService("start", "wuauserv");
                RunService("start", "bits");

                return OperationResult.Success("Cache Windows Update wyczyszczony.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Błąd czyszczenia cache WU: {ex.Message}");
                return OperationResult.Failure(ex.Message, ex);
            }
        });
    }

    public async Task<OperationResult> CleanRecycleBinAsync()
    {
        return await Task.Run(() =>
        {
            _logger.LogInfo("Opróżnianie Kosza...");

            const uint SHERB_NOCONFIRMATION = 0x00000001;
            const uint SHERB_NOPROGRESSUI   = 0x00000002;
            const uint SHERB_NOSOUND        = 0x00000004;

            int wynik = SHEmptyRecycleBin(
                IntPtr.Zero, null,
                SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);

            bool juzPusty   = wynik == unchecked((int)0x80070057);
            bool niemaKosza = wynik == unchecked((int)0x8000FFFF);

            if (wynik == 0 || juzPusty || niemaKosza)
            {
                _logger.LogSuccess("Kosz opróżniony (lub był już pusty).");
                return OperationResult.Success("Kosz opróżniony.");
            }

            _logger.LogWarning($"Kosz: kod 0x{wynik:X8} — pominięto.");
            return OperationResult.Success($"Kosz: pominięto (0x{wynik:X8}).");
        });
    }

    private async Task<OperationResult> CzyscFolderAsync(string sciezka, string nazwa)
    {
        if (!Directory.Exists(sciezka))
        {
            _logger.LogWarning($"{nazwa}: folder nie istnieje — {sciezka}");
            return OperationResult.Success($"{nazwa}: pominięty (nie istnieje).");
        }

        return await Task.Run(() =>
        {
            _logger.LogInfo($"Czyszczenie: {nazwa}...");
            int usunieto = UsunZawartoscFolderu(sciezka);
            _logger.LogSuccess($"{nazwa}: usunięto {usunieto} elementów.");
            return OperationResult.Success($"Usunięto {usunieto} elementów z {nazwa}.");
        });
    }

    private int UsunZawartoscFolderu(string sciezka)
    {
        int licznik = 0;

        foreach (string plik in Directory.GetFiles(sciezka))
            SprobujUsunPlik(plik, ref licznik);

        foreach (string folder in Directory.GetDirectories(sciezka))
            SprobujUsunFolder(folder, ref licznik);

        return licznik;
    }

    private void SprobujUsunPlik(string sciezka, ref int licznik)
    {
        try
        {
            File.SetAttributes(sciezka, FileAttributes.Normal);
            File.Delete(sciezka);
            licznik++;
        }
        catch { }
    }

    private void SprobujUsunFolder(string sciezka, ref int licznik)
    {
        try
        {
            Directory.Delete(sciezka, recursive: true);
            licznik++;
        }
        catch { }
    }

    private static void RunService(string komenda, string usluga)
    {
        try
        {
            var info = new ProcessStartInfo(SystemPaths.Sc, $"{komenda} {usluga}")
            {
                CreateNoWindow  = true,
                UseShellExecute = false
            };
            using var proc = Process.Start(info);
            proc?.WaitForExit(5000);
        }
        catch { }
    }

    [DllImport("Shell32.dll")]
    private static extern int SHEmptyRecycleBin(IntPtr hwnd, string? pszRootPath, uint dwFlags);
}
