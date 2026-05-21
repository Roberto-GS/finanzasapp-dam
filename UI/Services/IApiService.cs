using FinanzasApp.Core.DTOs.Autentificacion;
using FinanzasApp.Core.DTOs.Categoria;
using FinanzasApp.Core.DTOs.Grupo;
using FinanzasApp.Core.DTOs.Movimiento;
using FinanzasApp.Core.DTOs.Notificacion;
using FinanzasApp.Core.DTOs.Objetivo;
using FinanzasApp.Core.DTOs.Usuario;

namespace FinanzasApp.UI.Services;

public interface IApiService
{
    // Auth
    Task<AuthResponseDto> Login(LoginDto dto);
    Task<AuthResponseDto> Registro(RegistroDto dto);

    // Movimientos
    Task<List<MovimientoDto>> ObtenerMovimientos();
    Task<List<MovimientoDto>> ObtenerMovimientosPorMes(int anio, int mes);
    Task<List<MovimientoDto>> ObtenerUltimosMovimientos(int cantidad = 5);
    Task<List<MovimientoDto>> ObtenerMovimientosPorTipo(string tipo);
    Task<ResumenMesDto> ObtenerResumenMes(int anio, int mes);
    Task<MovimientoDto> CrearMovimiento(CrearMovimientoDto dto);
    Task<MovimientoDto> EditarMovimiento(int id, EditarMovimientoDto dto);
    Task EliminarMovimiento(int id);
    Task<List<GraficaCategoriaDto>> ObtenerGraficaCategorias(string tipo, int anio, int mes);
    Task<List<GraficaMensualDto>> ObtenerGraficaMensual(string tipo, int anio);
    Task<List<MovimientoDto>> BuscarMovimientos(string? texto, int? anio, int? mes, int? categoriaId, string? tipo);

    // Categorias
    Task<List<CategoriaDto>> ObtenerCategorias();
    Task<CategoriaDto> CrearCategoria(CrearCategoriaDto dto);
    Task EliminarCategoria(int id);

    // Objetivos
    Task<List<ObjetivoDto>> ObtenerObjetivos();
    Task<List<ObjetivoDto>> ObtenerObjetivosProximos(int dias = 3);
    Task<ObjetivoDto> CrearObjetivo(CrearObjetivoDto dto);
    Task<ObjetivoDto> EditarObjetivo(int id, CrearObjetivoDto dto);
    Task EliminarObjetivo(int id);
    Task<List<TipoObjetivoDto>> ObtenerTiposObjetivo();
    Task<ObjetivoDto> ObtenerObjetivoPorId(int id);
    Task<List<ObjetivoConProgresoDto>> ObtenerObjetivosConProgreso();
    Task<ObjetivoConProgresoDto> ObtenerObjetivoConProgreso(int id);

    // Notificaciones
    Task<List<NotificacionDto>> ObtenerNotificaciones();
    Task EliminarNotificacion(int id);
    Task EliminarTodasNotificaciones();

    // Usuario
    Task CambiarPassword(int usuarioId, string passwordActual, string passwordNueva);

    // Grupos
    Task<List<GrupoDto>> ObtenerGrupos();
    Task<GrupoDto> CrearGrupo(CrearGrupoDto dto);
    Task EliminarGrupo(int id);
    Task<ResumenGrupoDto> ObtenerResumenGrupo(int id);
    Task InvitarUsuario(int grupoId, string emailInvitado);
    Task<List<SolicitudGrupoDto>> ObtenerSolicitudesPendientes();
    Task AceptarSolicitud(int solicitudId);
    Task RechazarSolicitud(int solicitudId);
}