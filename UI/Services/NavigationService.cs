using FinanzasApp.UI.Views;

namespace FinanzasApp.UI.Services;

public class NavigationService : INavigationService
{
    /// <inheritdoc/>
    public async Task Navegar(string ruta)
        => await Shell.Current.GoToAsync(ruta);

    /// <inheritdoc/>
    public async Task Navegar(string ruta, IDictionary<string, object> parametros)
        => await Shell.Current.GoToAsync(ruta, parametros);

    /// <inheritdoc/>
    public async Task NavigarAtras()
        => await Shell.Current.GoToAsync("..");

    /// <inheritdoc/>
    public async Task NavegarALogin()
    {
        // Sustituimos la página raíz por el login al cerrar sesión
        NavigationPage loginPage = new NavigationPage(
            IPlatformApplication.Current!.Services
                .GetRequiredService<LoginPage>());

        if (Application.Current is not null)
            Application.Current.Windows[0].Page = loginPage;
    }

    /// <inheritdoc/>
    public async Task NavegarAShell()
    {
        // Sustituimos la página raíz por el Shell tras iniciar sesión
        if (Application.Current is not null)
            Application.Current.Windows[0].Page = new AppShell();

        await Task.CompletedTask;
    }
}