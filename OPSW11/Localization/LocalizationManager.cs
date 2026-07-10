using System.ComponentModel;
using OPSW11.Services;

namespace OPSW11.Localization;

/// <summary>
/// Runtime localization source. XAML binds to the string indexer via the
/// <see cref="LocExtension"/> markup extension; changing the language raises
/// <c>PropertyChanged("Item[]")</c> which refreshes every bound string live —
/// no window reload required.
/// </summary>
public sealed class LocalizationManager : INotifyPropertyChanged
{
    public static LocalizationManager Instance { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    private string _language;

    /// <summary>Languages offered in the UI, in display order.</summary>
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

    /// <summary>Indexer used by XAML bindings: <c>{loc:Loc SomeKey}</c>.</summary>
    public string this[string key] => Translations.Get(_language, key);

    public string Format(string key, params object[] args)
        => string.Format(Translations.Get(_language, key), args);

    public void SetLanguage(string code)
    {
        if (!Translations.IsSupported(code) || code == _language) return;

        _language = code;
        SettingsService.Current.Language = code;
        SettingsService.Save();

        // Refresh all indexer bindings + anyone watching the current language.
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLanguage)));
        LanguageChanged?.Invoke();
    }

    /// <summary>Fired after the language changes, for code-behind that needs to re-render.</summary>
    public event Action? LanguageChanged;
}

public sealed record LanguageOption(string Code, string ShortLabel, string NativeName);

/// <summary>Convenience facade for code-behind string lookups.</summary>
public static class Loc
{
    public static string T(string key) => LocalizationManager.Instance[key];

    public static string F(string key, params object[] args)
        => LocalizationManager.Instance.Format(key, args);
}
