using FinanzasApp.UI.Views;

namespace FinanzasApp.UI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Rutas para páginas de detalle (no están en el tabs)
        Routing.RegisterRoute(nameof(CrearMovimientoPage), typeof(CrearMovimientoPage));
        Routing.RegisterRoute(nameof(DetalleMovimientoPage), typeof(DetalleMovimientoPage));
        Routing.RegisterRoute(nameof(CrearCategoriaPage), typeof(CrearCategoriaPage));
        Routing.RegisterRoute(nameof(CrearObjetivoPage), typeof(CrearObjetivoPage));
        Routing.RegisterRoute(nameof(DetalleObjetivoPage), typeof(DetalleObjetivoPage));
        Routing.RegisterRoute(nameof(RegistroPage), typeof(RegistroPage));
        Routing.RegisterRoute(nameof(DetalleCategoriaPage), typeof(DetalleCategoriaPage));
        Routing.RegisterRoute(nameof(GraficasPage), typeof(GraficasPage));
        Routing.RegisterRoute(nameof(BuscarMovimientosPage), typeof(BuscarMovimientosPage));
        Routing.RegisterRoute(nameof(DetalleGrupoPage), typeof(DetalleGrupoPage));
        Routing.RegisterRoute(nameof(SolicitudesGrupoPage), typeof(SolicitudesGrupoPage));
        Routing.RegisterRoute(nameof(NotificacionesPage), typeof(NotificacionesPage));
    }
}