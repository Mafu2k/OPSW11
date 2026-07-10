using System.Windows.Data;
using System.Windows.Markup;

namespace OPSW11.Localization;

/// <summary>
/// XAML markup extension that binds a property to a localized string:
/// <c>Text="{loc:Loc QuickFix_Title}"</c>. Because it produces a real
/// <see cref="Binding"/> against <see cref="LocalizationManager"/>'s indexer,
/// the text updates automatically when the language is switched at runtime.
/// </summary>
[MarkupExtensionReturnType(typeof(string))]
public class LocExtension : MarkupExtension
{
    public string Key { get; set; } = string.Empty;

    public LocExtension() { }

    public LocExtension(string key) => Key = key;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var binding = new Binding($"[{Key}]")
        {
            Source = LocalizationManager.Instance,
            Mode = BindingMode.OneWay
        };

        return binding.ProvideValue(serviceProvider);
    }
}
