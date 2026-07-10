using System.IO;
using System.Text.Json;

namespace OPSW11.Services;

public class AppSettings
{
    public string Language { get; set; } = "pl";
    public string Theme { get; set; } = "dark";
}

public static class SettingsService
{
    private static readonly string Dir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OPSW11");

    private static readonly string FilePath = Path.Combine(Dir, "settings.json");

    private static AppSettings? _cached;

    public static AppSettings Current => _cached ??= Load();

    private static AppSettings Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                var loaded = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath));
                if (loaded is not null) return loaded;
            }
        }
        catch
        {
        }

        return new AppSettings();
    }

    public static void Save()
    {
        try
        {
            Directory.CreateDirectory(Dir);
            File.WriteAllText(
                FilePath,
                JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch
        {
        }
    }
}
