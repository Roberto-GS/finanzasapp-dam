using CommunityToolkit.Maui;
using FinanzasApp.UI.Services;
using FinanzasApp.UI.ViewModels;
using FinanzasApp.UI.Views;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using UI;

namespace FinanzasApp.UI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseSkiaSharp()
            .UseMicrocharts()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
            });

        // Servicios
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<ISessionService, SessionService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegistroViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<MovimientosViewModel>();
        builder.Services.AddTransient<CategoriasViewModel>();
        builder.Services.AddTransient<ObjetivosViewModel>();
        builder.Services.AddTransient<CrearMovimientoViewModel>();
        builder.Services.AddTransient<DetalleMovimientoViewModel>();
        builder.Services.AddTransient<CrearCategoriaViewModel>();
        builder.Services.AddTransient<CrearObjetivoViewModel>();
        builder.Services.AddTransient<DetalleObjetivoViewModel>();
        builder.Services.AddTransient<DetalleCategoriaViewModel>();
        builder.Services.AddTransient<AjustesViewModel>();
        builder.Services.AddTransient<GraficasViewModel>();
        builder.Services.AddTransient<BuscarMovimientosViewModel>();
        builder.Services.AddTransient<GruposViewModel>();
        builder.Services.AddTransient<DetalleGrupoViewModel>();
        builder.Services.AddTransient<SolicitudesGrupoViewModel>();
        builder.Services.AddTransient<NotificacionesViewModel>();

        // Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegistroPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<MovimientosPage>();
        builder.Services.AddTransient<CategoriasPage>();
        builder.Services.AddTransient<ObjetivosPage>();
        builder.Services.AddTransient<CrearMovimientoPage>();
        builder.Services.AddTransient<DetalleMovimientoPage>();
        builder.Services.AddTransient<CrearCategoriaPage>();
        builder.Services.AddTransient<CrearObjetivoPage>();
        builder.Services.AddTransient<DetalleObjetivoPage>();
        builder.Services.AddTransient<DetalleCategoriaPage>();
        builder.Services.AddTransient<AjustesPage>();
        builder.Services.AddTransient<GraficasPage>();
        builder.Services.AddTransient<BuscarMovimientosPage>();
        builder.Services.AddTransient<GruposPage>();
        builder.Services.AddTransient<DetalleGrupoPage>();
        builder.Services.AddTransient<SolicitudesGrupoPage>();
        builder.Services.AddTransient<NotificacionesPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}