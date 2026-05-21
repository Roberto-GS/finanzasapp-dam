using FinanzasApp.UI.Services;
using FinanzasApp.UI.ViewModels;
using FinanzasApp.UI.Views;
using UI;

namespace FinanzasApp.UI;

public partial class App : Application
{
    private readonly ISessionService _sessionService;

    public App(ISessionService sessionService)
    {
        InitializeComponent();
        _sessionService = sessionService;

        string temaGuardado = Preferences.Default.Get("tema_app", "sistema");
        UserAppTheme = temaGuardado switch
        {
            "claro" => AppTheme.Light,
            "oscuro" => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        bool sesionActiva = _sessionService.HaySesionActiva();

        Page paginaInicial = sesionActiva
            ? new AppShell()
            : new NavigationPage(new LoginPage(
                IPlatformApplication.Current!.Services.GetRequiredService<LoginViewModel>()));

        return new Window(paginaInicial);
    }
}