using System.ComponentModel;
using OPSW11.Services;

namespace OPSW11.Localization;

public sealed class LocalizationManager : INotifyPropertyChanged
{
    public static LocalizationManager Instance { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    private string _language;

    public static IReadOnlyList<LanguageOption> Languages { get; } = new[]
    {
        new LanguageOption("pl", "PL", "Polski"),
        new LanguageOption("en", "EN", "English"),
        new LanguageOption("de", "DE", "Deutsch"),
        new LanguageOption("es", "ES", "Español"),
        new LanguageOption("fr", "FR", "Français"),
        new LanguageOption("uk", "UK", "Українська"),
    };

    private LocalizationManager()
    {
        var saved = SettingsService.Current.Language;
        _language = Translations.IsSupported(saved) ? saved : "pl";
    }

    public string CurrentLanguage => _language;

    public string this[string key] => Translations.Get(_language, key);

    public string Format(string key, params object[] args)
        => string.Format(Translations.Get(_language, key), args);

    public void SetLanguage(string code)
    {
        if (!Translations.IsSupported(code) || code == _language) return;

        _language = code;
        SettingsService.Current.Language = code;
        SettingsService.Save();

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLanguage)));
        LanguageChanged?.Invoke();
    }

    public event Action? LanguageChanged;
}

public sealed record LanguageOption(string Code, string ShortLabel, string NativeName);

public static class Loc
{
    public static string T(string key) => LocalizationManager.Instance[key];

    public static string F(string key, params object[] args)
        => LocalizationManager.Instance.Format(key, args);
}
