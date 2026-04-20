using System.IO;
using System.Management;
using System.ServiceProcess;
using System.Text.Json;
using OPSW11.Models;

namespace OPSW11.Services;

public class BackupService
{
    private readonly LoggingService _logger;
    private readonly string _backupDirectory;

    public BackupService(LoggingService logger)
    {
        _logger = logger;
        _backupDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "WO11", "Backups");

        try
        {
            Directory.CreateDirectory(_backupDirectory);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Nie można utworzyć katalogu backupów: {ex.Message}");
        }
    }

    public async Task<OperationResult> CreateSystemRestorePointAsync(string description)
    {
        return await Task.Run(() =>
        {
            _logger.LogInfo($"Creating system restore point: \"{description}\"...");

            try
            {
                using var restoreClass = new ManagementClass(
                    @"\\localhost\root\default", "SystemRestore",
                    new ObjectGetOptions());

                using var inParams = restoreClass.GetMethodParameters("CreateRestorePoint");
                inParams["Description"]      = description;
                inParams["RestorePointType"] = 12;
                inParams["EventType"]        = 100;

                using var result = restoreClass.InvokeMethod("CreateRestorePoint", inParams, null);
                int returnCode = Convert.ToInt32(result["ReturnValue"]);

                if (returnCode == 0)
                {
                    _logger.LogSuccess("System restore point created.");
                    return OperationResult.Success("System restore point created.");
                }

                _logger.LogWarning($"Restore point returned code {returnCode} (may still succeed).");
                return OperationResult.Success($"Restore point requested (code {returnCode}).");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not create restore point: {ex.Message}");
                return OperationResult.Failure($"Restore point failed: {ex.Message}", ex);
            }
        });
    }

    public async Task<OperationResult> BackupServiceStatesAsync(string[] serviceNames)
    {
        return await Task.Run(() =>
        {
            _logger.LogInfo("Backing up current service states...");

            try
            {
                var states = new Dictionary<string, string>();

                foreach (string name in serviceNames)
                {
                    try
                    {
                        using var sc = new ServiceController(name);
                        states[name] = sc.Status.ToString();
                    }
                    catch (InvalidOperationException) { }
                }

                string timestamp  = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string backupPath = Path.Combine(_backupDirectory, $"services_{timestamp}.json");

                File.WriteAllText(backupPath, JsonSerializer.Serialize(
                    states, new JsonSerializerOptions { WriteIndented = true }));

                _logger.LogSuccess($"Service states backed up → {backupPath}");
                return OperationResult.Success($"Backed up {states.Count} service states.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Service backup failed: {ex.Message}");
                return OperationResult.Failure(ex.Message, ex);
            }
        });
    }

    public string BackupDirectory => _backupDirectory;
}
