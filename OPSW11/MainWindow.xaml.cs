using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using OPSW11.Helpers;
using OPSW11.Views;

namespace OPSW11;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _zegar = new() { Interval = TimeSpan.FromSeconds(1) };
    private bool _ciemnyMotyw = true;

    public MainWindow()
    {
        InitializeComponent();
        SprawdzAdmina();
        UstawZegar();
        Nawiguj("Dashboard");
    }

    private void ThemeToggle_Click(object sender, RoutedEventArgs e)
    {
        _ciemnyMotyw = !_ciemnyMotyw;

        var slowniki = Application.Current.Resources.MergedDictionaries;
        var aktualny = slowniki.FirstOrDefault(d => d.Source?.OriginalString.Contains("Colors") == true);
        if (aktualny != null) slowniki.Remove(aktualny);

        string plik = _ciemnyMotyw ? "DarkColors.xaml" : "LightColors.xaml";
        slowniki.Insert(0, new System.Windows.ResourceDictionary
        {
            Source = new Uri($"pack://application:,,,/Resources/{plik}")
        });

        ThemeToggleButton.Content = _ciemnyMotyw ? "☀  Jasny motyw" : "🌙  Ciemny motyw";
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

        string etykieta;
        switch (widok)
        {
            case "Dashboard":
                MainContent.Content  = new DashboardView();
                BtnDashboard.Style   = (Style)FindResource("NavButtonActive");
                etykieta = "Panel główny";
                break;

            case "QuickFix":
                MainContent.Content  = new QuickFixView();
                BtnQuickFix.Style    = (Style)FindResource("NavButtonActive");
                etykieta = "Szybka naprawa";
                break;

            case "AdvancedFix":
                MainContent.Content  = new AdvancedFixView();
                BtnAdvancedFix.Style = (Style)FindResource("NavButtonActive");
                etykieta = "Zaawansowana naprawa";
                break;

            case "CustomFix":
                MainContent.Content  = new CustomFixView();
                BtnCustomFix.Style   = (Style)FindResource("NavButtonActive");
                etykieta = "Naprawa własna";
                break;

            case "Logs":
                MainContent.Content  = new LogsView();
                BtnLogs.Style        = (Style)FindResource("NavButtonActive");
                etykieta = "Dziennik";
                break;

            default:
                return;
        }

        StatusBarMessage.Text = $"Widok: {etykieta}";
    }

    private void SprawdzAdmina()
    {
        bool jestAdmin = AdminHelper.IsRunningAsAdministrator();

        AdminBadgeText.Text    = jestAdmin ? "Tryb administratora" : "Brak uprawnień admina";
        AdminBadge.BorderBrush = jestAdmin
            ? (System.Windows.Media.Brush)FindResource("AccentBrush")
            : (System.Windows.Media.Brush)FindResource("WarningBrush");
    }

    private void UstawZegar()
    {
        _zegar.Tick += (_, _) =>
            StatusBarClock.Text = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");
        _zegar.Start();

        StatusBarClock.Text = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");
    }
}