using AutoMapper;
using FinanzasApp.Core.DTOs.Movimiento;
using FinanzasApp.API.Extensions;
using FinanzasApp.Core.Enums;
using FinanzasApp.Core.Interfaces;
using FinanzasApp.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanzasApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MovimientosController : ControllerBase
{
    private readonly IMovimientoRepository _movimientoRepository;
    private readonly IMapper _mapper;

    public MovimientosController(IMovimientoRepository movimientoRepository, IMapper mapper)
    {
        _movimientoRepository = movimientoRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtenemos todos los movimientos del usuario
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        int usuarioId = User.ObtenerUsuarioId();
        IEnumerable<Movimiento> movimientos = await _movimientoRepository.ObtenerPorIdUsuario(usuarioId);
        return Ok(_mapper.Map<IEnumerable<MovimientoDto>>(movimientos));
    }

    /// <summary>
    /// Obtenemos un movimiento concreto por su Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        int usuarioId = User.ObtenerUsuarioId();
        Movimiento movimiento = await _movimientoRepository.ObtenerPorID(id)
            ?? throw new KeyNotFoundException("Movimiento no encontrado");

        if (movimiento.UsuarioId != usuarioId)
            return Forbid();

        return Ok(_mapper.Map<MovimientoDto>(movimiento));
    }

    /// <summary>
    /// Obtenemos los movimientos de un usuario filtrados por año y por mes
    /// </summary>
    /// <param name="anio"></param>
    /// <param name="mes"></param>
    /// <returns></returns>
    [HttpGet("mes/{anio:int}/{mes:int}")]
    public async Task<IActionResult> ObtenerPorMes(int anio, int mes)
    {
        if (mes < 1 || mes > 12)
            return BadRequest("El mes introducido debe estar entre 1 y 12");

        if (anio > DateTime.UtcNow.Year ||
           (anio == DateTime.UtcNow.Year && mes > DateTime.UtcNow.Month))
            return BadRequest("No se pueden consultar meses futuros");

        int usuarioId = User.ObtenerUsuarioId();
        IEnumerable<Movimiento> movimientos = await _movimientoRepository.ObtenerPorUsuarioYMes(usuarioId, anio, mes);
        return Ok(_mapper.Map<IEnumerable<MovimientoDto>>(movimientos));
    }


    /// <summary>
    /// Obtenemos un resumen financiero de un mes concreto, con el total de ingresos, el total de gastos y el balance
    /// </summary>
    /// <param name="anio"></param>
    /// <param name="mes"></param>
    /// <returns></returns>
    [HttpGet("resumen/{anio:int}/{mes:int}")]
    public async Task<IActionResult> ObtenerResumenMes(int anio, int mes)
    {
        if (mes < 1 || mes > 12)
            return BadRequest("El mes introducido debe estar entre 1 y 12");

        if (anio > DateTime.UtcNow.Year ||
           (anio == DateTime.UtcNow.Year && mes > DateTime.UtcNow.Month))
            return BadRequest("No se pueden consultar meses futuros");

        int usuarioId = User.ObtenerUsuarioId();

        decimal totalIngresos = await _movimientoRepository
            .ObtenerTotalPorTipoYMes(usuarioId, TipoMovimiento.Ingreso, anio, mes);

        decimal totalGastos = await _movimientoRepository
            .ObtenerTotalPorTipoYMes(usuarioId, TipoMovimiento.Gasto, anio, mes);

        ResumenMesDto resumen = new ResumenMesDto
        {
            Anio = anio,
            Mes = mes,
            TotalIngresos = totalIngresos,
            TotalGastos = totalGastos
        };

        return Ok(resumen);
    }

    /// <summary>
    /// Obtenemos una determinada cantidad de los ultimos movimientos de un usuario
    /// </summary>
    /// <param name="cantidad"></param>
    /// <returns></returns>
    [HttpGet("ultimos/{cantidad:int}")]
    public async Task<IActionResult> ObtenerUltimos(int cantidad = 5)
    {
        if (cantidad < 1 || cantidad > 50)
            return BadRequest("La cantidad debe estar entre 1 y 50");

        int usuarioId = User.ObtenerUsuarioId();
        IEnumerable<Movimiento> movimientos = await _movimientoRepository.ObtenerUltimos(usuarioId, cantidad);
        return Ok(_mapper.Map<IEnumerable<MovimientoDto>>(movimientos));
    }

    /// <summary>
    /// Obtenemos los movimientos del usuario filtrados por el tipo
    /// </summary>
    /// <param name="tipo"></param>
    /// <returns></returns>
    [HttpGet("tipo/{tipo}")]
    public async Task<IActionResult> ObtenerPorTipo(TipoMovimiento tipo)
    {
        int usuarioId = User.ObtenerUsuarioId();
        IEnumerable<Movimiento> movimientos = await _movimientoRepository.ObtenerPorTipo(usuarioId, tipo);
        return Ok(_mapper.Map<IEnumerable<MovimientoDto>>(movimientos));
    }

    /// <summary>
    /// Creamos un nuevo movimiento para el usuario. El Id y la Fecha de creación se generan automáticamente al crear el movimiento
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearMovimientoDto dto)
    {
        int usuarioId = User.ObtenerUsuarioId();
        Movimiento movimiento = _mapper.Map<Movimiento>(dto);
        movimiento.UsuarioId = usuarioId;

        await _movimientoRepository.Agregar(movimiento);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = movimiento.Id },
            _mapper.Map<MovimientoDto>(movimiento));
    }

    /// <summary>
    /// Actualizamos los datos de un movimiento
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, [FromBody] EditarMovimientoDto dto)
    {
        int usuarioId = User.ObtenerUsuarioId();
        Movimiento movimiento = await _movimientoRepository.ObtenerPorID(id)
            ?? throw new KeyNotFoundException("Movimiento no encontrado");

        if (movimiento.UsuarioId != usuarioId)
            return Forbid();

        _mapper.Map(dto, movimiento);
        await _movimientoRepository.Actualizar(movimiento);
        return Ok(_mapper.Map<MovimientoDto>(movimiento));
    }

    /// <summary>
    /// Eliminamos un movimiento del usuario
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        int usuarioId = User.ObtenerUsuarioId();
        Movimiento movimiento = await _movimientoRepository.ObtenerPorID(id)
            ?? throw new KeyNotFoundException("Movimiento no encontrado");

        if (movimiento.UsuarioId != usuarioId)
            return Forbid();

        await _movimientoRepository.Eliminar(id);
        return NoContent();
    }

    /// <summary>
    /// Obtenemos los datos para la gráfica circular de distribución de gastos o ingresos por categoría en un mes concreto
    /// </summary>
    /// <param name="tipo"></param>
    /// <param name="anio"></param>
    /// <param name="mes"></param>
    /// <returns></returns>
    [HttpGet("grafica/categorias/{tipo}/{anio:int}/{mes:int}")]
    public async Task<IActionResult> ObtenerGraficaCategorias(TipoMovimiento tipo, int anio, int mes)
    {
        if (mes < 1 || mes > 12)
            return BadRequest("El mes introducido debe estar entre 1 y 12");

        if (anio > DateTime.UtcNow.Year ||
           (anio == DateTime.UtcNow.Year && mes > DateTime.UtcNow.Month))
            return BadRequest("No se pueden consultar meses futuros");

        int usuarioId = User.ObtenerUsuarioId();
        IEnumerable<Movimiento> movimientos = await _movimientoRepository.ObtenerPorUsuarioMesTipo(usuarioId, anio, mes, tipo);

        decimal total = movimientos.Sum(m => m.Cantidad);

        List<GraficaCategoriaDto> resultado = movimientos
            .GroupBy(m => new
            {
                Nombre = m.Categoria?.Nombre ?? "Sin categoría",
                Color = m.Categoria?.Color ?? "#6B7280"
            })
            .Select(g => new GraficaCategoriaDto
            {
                CategoriaNombre = g.Key.Nombre,
                CategoriaColor = g.Key.Color,
                Total = g.Sum(m => m.Cantidad),
                Porcentaje = total > 0
                    ? Math.Round((double)(g.Sum(m => m.Cantidad) / total * 100), 1)
                    : 0
            })
            .OrderByDescending(g => g.Total)
            .ToList();

        return Ok(resultado);
    }

    /// <summary>
    /// Obtenemos los datos para la gráfica de barras de evolución mensual de gastos o ingresos a lo largo de un año
    /// </summary>
    /// <param name="tipo"></param>
    /// <param name="anio"></param>
    /// <returns></returns>
    [HttpGet("grafica/mensual/{tipo}/{anio:int}")]
    public async Task<IActionResult> ObtenerGraficaMensual(TipoMovimiento tipo, int anio)
    {
        if (anio > DateTime.UtcNow.Year)
            return BadRequest("No se pueden consultar años futuros");

        int usuarioId = User.ObtenerUsuarioId();
        IEnumerable<Movimiento> movimientos = await _movimientoRepository.ObtenerPorUsuarioTipoYAnio(usuarioId, tipo, anio);

        string[] mesesNombres =
        [
            "Ene", "Feb", "Mar", "Abr", "May", "Jun",
            "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"
        ];

        // Solo mostramos hasta el mes actual si es el año en curso
        int mesActual = anio == DateTime.UtcNow.Year ? DateTime.UtcNow.Month : 12;

        List<GraficaMensualDto> resultado = Enumerable.Range(1, mesActual)
            .Select(mes => new GraficaMensualDto
            {
                Mes = mes,
                MesNombre = mesesNombres[mes - 1],
                Total = movimientos
                    .Where(m => m.Fecha.Month == mes)
                    .Sum(m => m.Cantidad)
            })
            .ToList();

        return Ok(resultado);
    }

    /// <summary>
    /// Realizamos una búsqueda de movimientos por texto, filtrando por año, mes, categoría y tipo. El texto se busca en el nombre, la descripción y la etiqueta del movimiento
    /// </summary>
    /// <param name="texto"></param>
    /// <param name="anio"></param>
    /// <param name="mes"></param>
    /// <param name="categoriaId"></param>
    /// <param name="tipo"></param>
    /// <returns></returns>
    [HttpGet("buscar")]
    public async Task<IActionResult> Buscar(
        [FromQuery] string? texto,
        [FromQuery] int? anio,
        [FromQuery] int? mes,
        [FromQuery] int? categoriaId,
        [FromQuery] TipoMovimiento? tipo)
    {
        int usuarioId = User.ObtenerUsuarioId();
        IEnumerable<Movimiento> movimientos = await _movimientoRepository
            .Buscar(usuarioId, texto, anio, mes, categoriaId, tipo);

        return Ok(_mapper.Map<IEnumerable<MovimientoDto>>(movimientos));
    }
}