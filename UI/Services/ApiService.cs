using FinanzasApp.Core.DTOs.Autentificacion;
using FinanzasApp.Core.DTOs.Categoria;
using FinanzasApp.Core.DTOs.Grupo;
using FinanzasApp.Core.DTOs.Movimiento;
using FinanzasApp.Core.DTOs.Notificacion;
using FinanzasApp.Core.DTOs.Objetivo;
using FinanzasApp.Core.DTOs.Usuario;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FinanzasApp.UI.Services;

public class ApiService : IApiService
{
    private readonly ISessionService _sessionService;
    private readonly INavigationService _navigationService;
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

#if ANDROID
    private const string BaseUrl = "http://192.168.1.XX:5281/api/";
#elif WINDOWS
    private const string BaseUrl = "https://localhost:7219/api/";
#else
    private const string BaseUrl = "https://localhost:7219/api/";
#endif

    public ApiService(ISessionService sessionService, INavigationService navigationService)
    {
        _sessionService = sessionService;
        _navigationService = navigationService;
        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
    }

    // Cabecera de autenticación
    private void AgregarToken()
    {
        string token = _sessionService.ObtenerToken();
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        else
            _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    // Manejo de errores HTTP
    private async Task ManejarError(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _sessionService.CerrarSesion();
            await _navigationService.NavegarALogin();
            throw new Exception("Sesión expirada. Por favor, inicia sesión de nuevo.");
        }

        if (!response.IsSuccessStatusCode)
        {
            string contenido = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error {(int)response.StatusCode}: {contenido}");
        }
    }

    // Métodos genéricos
    private async Task<T> GetAsync<T>(string endpoint)
    {
        AgregarToken();
        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
        await ManejarError(response);
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions)
               ?? throw new Exception("Respuesta vacía del servidor.");
    }

    private async Task<T> PostAsync<T>(string endpoint, object body)
    {
        AgregarToken();
        string json = JsonSerializer.Serialize(body, JsonOptions);
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);
        await ManejarError(response);
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions)
               ?? throw new Exception("Respuesta vacía del servidor.");
    }

    private async Task<T> PutAsync<T>(string endpoint, object body)
    {
        AgregarToken();
        string json = JsonSerializer.Serialize(body, JsonOptions);
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _httpClient.PutAsync(endpoint, content);
        await ManejarError(response);
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions)
               ?? throw new Exception("Respuesta vacía del servidor.");
    }

    private async Task DeleteAsync(string endpoint)
    {
        AgregarToken();
        HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint);
        await ManejarError(response);
    }

    // Auth
    public Task<AuthResponseDto> Login(LoginDto dto)
        => PostAsync<AuthResponseDto>("auth/login", dto);

    public Task<AuthResponseDto> Registro(RegistroDto dto)
        => PostAsync<AuthResponseDto>("auth/registro", dto);

    // Movimientos
    public Task<List<MovimientoDto>> ObtenerMovimientos()
        => GetAsync<List<MovimientoDto>>("movimientos");

    public Task<List<MovimientoDto>> ObtenerMovimientosPorMes(int anio, int mes)
        => GetAsync<List<MovimientoDto>>($"movimientos/mes/{anio}/{mes}");

    public Task<List<MovimientoDto>> ObtenerUltimosMovimientos(int cantidad = 5)
        => GetAsync<List<MovimientoDto>>($"movimientos/ultimos/{cantidad}");

    public Task<List<MovimientoDto>> ObtenerMovimientosPorTipo(string tipo)
        => GetAsync<List<MovimientoDto>>($"movimientos/tipo/{tipo}");

    public Task<ResumenMesDto> ObtenerResumenMes(int anio, int mes)
        => GetAsync<ResumenMesDto>($"movimientos/resumen/{anio}/{mes}");

    public Task<MovimientoDto> CrearMovimiento(CrearMovimientoDto dto)
        => PostAsync<MovimientoDto>("movimientos", dto);

    public Task<MovimientoDto> EditarMovimiento(int id, EditarMovimientoDto dto)
        => PutAsync<MovimientoDto>($"movimientos/{id}", dto);

    public async Task EliminarMovimiento(int id)
        => await DeleteAsync($"movimientos/{id}");

    public Task<List<GraficaCategoriaDto>> ObtenerGraficaCategorias(
        string tipo, int anio, int mes)
        => GetAsync<List<GraficaCategoriaDto>>(
            $"movimientos/grafica/categorias/{tipo}/{anio}/{mes}");

    public Task<List<GraficaMensualDto>> ObtenerGraficaMensual(string tipo, int anio)
        => GetAsync<List<GraficaMensualDto>>($"movimientos/grafica/mensual/{tipo}/{anio}");

    public async Task<List<MovimientoDto>> BuscarMovimientos(
        string? texto, int? anio, int? mes, int? categoriaId, string? tipo)
    {
        List<string> parametros = new List<string>();
        if (!string.IsNullOrWhiteSpace(texto))
            parametros.Add($"texto={Uri.EscapeDataString(texto)}");
        if (anio.HasValue) parametros.Add($"anio={anio}");
        if (mes.HasValue) parametros.Add($"mes={mes}");
        if (categoriaId.HasValue) parametros.Add($"categoriaId={categoriaId}");
        if (!string.IsNullOrWhiteSpace(tipo)) parametros.Add($"tipo={tipo}");

        string query = parametros.Count > 0
            ? "?" + string.Join("&", parametros)
            : string.Empty;

        return await GetAsync<List<MovimientoDto>>($"movimientos/buscar{query}");
    }

    // Categorias
    public Task<List<CategoriaDto>> ObtenerCategorias()
        => GetAsync<List<CategoriaDto>>("categorias");

    public Task<CategoriaDto> CrearCategoria(CrearCategoriaDto dto)
        => PostAsync<CategoriaDto>("categorias", dto);

    public async Task EliminarCategoria(int id)
        => await DeleteAsync($"categorias/{id}");

    // Objetivos
    public Task<List<ObjetivoDto>> ObtenerObjetivos()
        => GetAsync<List<ObjetivoDto>>("objetivos");

    public Task<List<ObjetivoDto>> ObtenerObjetivosProximos(int dias = 3)
        => GetAsync<List<ObjetivoDto>>($"objetivos/proximos/{dias}");

    public Task<ObjetivoDto> CrearObjetivo(CrearObjetivoDto dto)
        => PostAsync<ObjetivoDto>("objetivos", dto);

    public Task<ObjetivoDto> EditarObjetivo(int id, CrearObjetivoDto dto)
        => PutAsync<ObjetivoDto>($"objetivos/{id}", dto);

    public async Task EliminarObjetivo(int id)
        => await DeleteAsync($"objetivos/{id}");

    public Task<List<TipoObjetivoDto>> ObtenerTiposObjetivo()
        => GetAsync<List<TipoObjetivoDto>>("objetivos/tipos");

    public Task<List<ObjetivoConProgresoDto>> ObtenerObjetivosConProgreso()
        => GetAsync<List<ObjetivoConProgresoDto>>("objetivos/con-progreso");

    public Task<ObjetivoConProgresoDto> ObtenerObjetivoConProgreso(int id)
        => GetAsync<ObjetivoConProgresoDto>($"objetivos/{id}/progreso");

    // Notificaciones
    public Task<List<NotificacionDto>> ObtenerNotificaciones()
        => GetAsync<List<NotificacionDto>>("notificaciones");

    public async Task EliminarNotificacion(int id)
        => await DeleteAsync($"notificaciones/{id}");

    public async Task EliminarTodasNotificaciones()
        => await DeleteAsync("notificaciones");

    // Usuario
    public async Task CambiarPassword(
        int usuarioId, string passwordActual, string passwordNueva)
    {
        CambiarPasswordDto dto = new CambiarPasswordDto
        {
            PasswordActual = passwordActual,
            PasswordNueva = passwordNueva
        };
        await PutAsync<object>("usuarios/cambiar-password", dto);
    }

    // Grupos
    public Task<List<GrupoDto>> ObtenerGrupos()
        => GetAsync<List<GrupoDto>>("grupos");

    public Task<GrupoDto> CrearGrupo(CrearGrupoDto dto)
        => PostAsync<GrupoDto>("grupos", dto);

    public async Task EliminarGrupo(int id)
        => await DeleteAsync($"grupos/{id}");

    public Task<ResumenGrupoDto> ObtenerResumenGrupo(int id)
        => GetAsync<ResumenGrupoDto>($"grupos/{id}/resumen");

    public async Task InvitarUsuario(int grupoId, string emailInvitado)
        => await PostAsync<object>($"grupos/{grupoId}/invitar",
            new InvitarUsuarioDto { EmailInvitado = emailInvitado });

    public Task<List<SolicitudGrupoDto>> ObtenerSolicitudesPendientes()
        => GetAsync<List<SolicitudGrupoDto>>("grupos/solicitudes");

    public async Task AceptarSolicitud(int solicitudId)
        => await PostAsync<object>(
            $"grupos/solicitudes/{solicitudId}/aceptar", new { });

    public async Task RechazarSolicitud(int solicitudId)
        => await PostAsync<object>(
            $"grupos/solicitudes/{solicitudId}/rechazar", new { });

    public Task<ObjetivoDto> ObtenerObjetivoPorId(int id)
    => GetAsync<ObjetivoDto>($"objetivos/{id}");
}