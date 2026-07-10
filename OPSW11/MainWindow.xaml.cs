using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using OPSW11.Helpers;
using OPSW11.Localization;
using OPSW11.Services;
using OPSW11.Views;

namespace OPSW11;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _zegar = new() { Interval = TimeSpan.FromSeconds(1) };
    private bool _ciemnyMotyw = true;
    private string _aktualnyWidok = "Dashboard";

    public MainWindow()
    {
        InitializeComponent();

        _ciemnyMotyw = SettingsService.Current.Theme != "light";
        ZastosujMotyw(_ciemnyMotyw);
        ZbudujPrzyciskiJezykow();

        LocalizationManager.Instance.LanguageChanged += OnLanguageChanged;

        SprawdzAdmina();
        UstawZegar();
        Nawiguj("Dashboard");
    }

    private void OnLanguageChanged()
    {
        SprawdzAdmina();
        AktualizujPrzyciskMotywu();
        AktualizujStatusWidoku();
        PodswietlAktywnyJezyk();
    }

    private void ZbudujPrzyciskiJezykow()
    {
        LangPanel.Children.Clear();

        foreach (var lang in LocalizationManager.Languages)
        {
            var btn = new Button
            {
                Content = lang.ShortLabel,
                Tag = lang.Code,
                Style = (Style)FindResource("SecondaryButton"),
                Padding = new Thickness(8, 5, 8, 5),
                Margin = new Thickness(0, 0, 6, 6),
                FontSize = 11,
                ToolTip = lang.NativeName,
                MinWidth = 40
            };
            btn.Click += (_, _) => LocalizationManager.Instance.SetLanguage(lang.Code);
            LangPanel.Children.Add(btn);
        }

        PodswietlAktywnyJezyk();
    }

    private void PodswietlAktywnyJezyk()
    {
        string aktywny = LocalizationManager.Instance.CurrentLanguage;

        foreach (var child in LangPanel.Children)
        {
            if (child is Button btn && btn.Tag is string code)
                btn.Style = (Style)FindResource(code == aktywny ? "PrimaryButton" : "SecondaryButton");
        }
    }

    private void ThemeToggle_Click(object sender, RoutedEventArgs e)
    {
        _ciemnyMotyw = !_ciemnyMotyw;
        ZastosujMotyw(_ciemnyMotyw);

        SettingsService.Current.Theme = _ciemnyMotyw ? "dark" : "light";
        SettingsService.Save();

        AktualizujPrzyciskMotywu();
    }

    private void ZastosujMotyw(bool ciemny)
    {
        var slowniki = Application.Current.Resources.MergedDictionaries;
        var aktualny = slowniki.FirstOrDefault(d => d.Source?.OriginalString.Contains("Colors") == true);
        if (aktualny != null) slowniki.Remove(aktualny);

        string plik = ciemny ? "DarkColors.xaml" : "LightColors.xaml";
        slowniki.Insert(0, new ResourceDictionary
        {
            Source = new Uri($"pack://application:,,,/Resources/{plik}")
        });

        AktualizujPrzyciskMotywu();
    }

    private void AktualizujPrzyciskMotywu()
    {
        if (ThemeToggleButton is null) return;

        ThemeToggleButton.Content = _ciemnyMotyw
            ? "☀  " + Loc.T("Theme_Light")
            : "🌙  " + Loc.T("Theme_Dark");
    }

    private void NavButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string tag)
            Nawiguj(tag);
    }

    private void Nawiguj(string widok)
    {
        foreach (var btn in new[] { BtnDashboard, BtnQuickFix, BtnAdvancedFix, BtnCustomFix, BtnLogs })
            btn.Style = (Style)FindResource("NavButton");

        switch (widok)
        {
            case "Dashboard":
                MainContent.Content = new DashboardView();
                BtnDashboard.Style  = (Style)FindResource("NavButtonActive");
                break;

            case "QuickFix":
                MainContent.Content = new QuickFixView();
                BtnQuickFix.Style   = (Style)FindResource("NavButtonActive");
                break;

            case "AdvancedFix":
                MainContent.Content  = new AdvancedFixView();
                BtnAdvancedFix.Style = (Style)FindResource("NavButtonActive");
                break;

            case "CustomFix":
                MainContent.Content = new CustomFixView();
                BtnCustomFix.Style  = (Style)FindResource("NavButtonActive");
                break;

            case "Logs":
                MainContent.Content = new LogsView();
                BtnLogs.Style       = (Style)FindResource("NavButtonActive");
                break;

            default:
                return;
        }

        _aktualnyWidok = widok;
        AktualizujStatusWidoku();
    }

    private void AktualizujStatusWidoku()
    {
        string key = _aktualnyWidok switch
        {
            "Dashboard"   => "Dash_Title",
            "QuickFix"    => "QF_Title",
            "AdvancedFix" => "AF_Title",
            "CustomFix"   => "CF_Title",
            "Logs"        => "Log_Title",
            _             => "Dash_Title"
        };

        StatusBarMessage.Text = Loc.F("Status_View", Loc.T(key));
    }

    private void SprawdzAdmina()
    {
        bool jestAdmin = AdminHelper.IsRunningAsAdministrator();

        AdminBadgeText.Text    = jestAdmin ? Loc.T("Admin_Yes") : Loc.T("Admin_No");
        AdminBadge.BorderBrush = jestAdmin
            ? (Brush)FindResource("AccentBrush")
            : (Brush)FindResource("WarningBrush");
    }

    private void UstawZegar()
    {
        _zegar.Tick += (_, _) =>
            StatusBarClock.Text = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");
        _zegar.Start();

        StatusBarClock.Text = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");
    }
}
