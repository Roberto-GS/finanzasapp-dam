using AutoMapper;
using FinanzasApp.API.Extensions;
using FinanzasApp.API.Services;
using FinanzasApp.Core.DTOs.Objetivo;
using FinanzasApp.Core.Interfaces;
using FinanzasApp.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanzasApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ObjetivosController : ControllerBase
{
    private readonly IObjetivoRepository _objetivoRepository;
    private readonly IMapper _mapper;
    private readonly IObjetivoService _objetivoService;

    public ObjetivosController(
        IObjetivoRepository objetivoRepository,
        IMapper mapper,
        IObjetivoService objetivoService)
    {
        _objetivoRepository = objetivoRepository;
        _mapper = mapper;
        _objetivoService = objetivoService;
    }

    /// <summary>
    /// Obtenemos todos los objetivos del usuario
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        int usuarioId = User.ObtenerUsuarioId();
        IEnumerable<Objetivo> objetivos = await _objetivoRepository.ObtenerPorUsuarioId(usuarioId);
        return Ok(_mapper.Map<IEnumerable<ObjetivoDto>>(objetivos));
    }

    /// <summary>
    /// Obtenemos un objetivo por su id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        int usuarioId = User.ObtenerUsuarioId();
        Objetivo objetivo = await _objetivoRepository.ObtenerPorID(id)
            ?? throw new KeyNotFoundException("Objetivo no encontrado");

        if (objetivo.UsuarioId != usuarioId)
            return Forbid();

        return Ok(_mapper.Map<ObjetivoDto>(objetivo));
    }

    /// <summary>
    /// Obtenemos los objetivos que vencen en los próximos días
    /// </summary>
    /// <param name="dias"></param>
    /// <returns></returns>
    [HttpGet("proximos/{dias:int}")]
    public async Task<IActionResult> ObtenerProximosAVencer(int dias = 3)
    {
        if (dias < 1 || dias > 30)
            return BadRequest("Los días deben estar entre 1 y 30");

        int usuarioId = User.ObtenerUsuarioId();
        IEnumerable<Objetivo> objetivos = await _objetivoRepository.ObtenerProximosAVencer(usuarioId, dias);
        return Ok(_mapper.Map<IEnumerable<ObjetivoDto>>(objetivos));
    }

    /// <summary>
    /// Creamos un nuevo objetivo financiero para el usuario
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearObjetivoDto dto)
    {
        int usuarioId = User.ObtenerUsuarioId();
        Objetivo objetivo = _mapper.Map<Objetivo>(dto);
        objetivo.UsuarioId = usuarioId;

        await _objetivoRepository.Agregar(objetivo);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = objetivo.Id },
            _mapper.Map<ObjetivoDto>(objetivo));
    }

    /// <summary>
    /// Actualizamos los datos de un objetivo
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, [FromBody] CrearObjetivoDto dto)
    {
        int usuarioId = User.ObtenerUsuarioId();
        Objetivo objetivo = await _objetivoRepository.ObtenerPorID(id)
            ?? throw new KeyNotFoundException("Objetivo no encontrado");

        if (objetivo.UsuarioId != usuarioId)
            return Forbid();

        _mapper.Map(dto, objetivo);
        await _objetivoRepository.Actualizar(objetivo);
        return Ok(_mapper.Map<ObjetivoDto>(objetivo));
    }

    /// <summary>
    /// Eliminamos un objetivo del usuario
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        int usuarioId = User.ObtenerUsuarioId();
        Objetivo objetivo = await _objetivoRepository.ObtenerPorID(id)
            ?? throw new KeyNotFoundException("Objetivo no encontrado");

        if (objetivo.UsuarioId != usuarioId)
            return Forbid();

        await _objetivoRepository.Eliminar(id);
        return NoContent();
    }

    /// <summary>
    /// Devuelve todos los objetivos con su progreso calculado en tiempo real
    /// </summary>
    /// <returns></returns>
    [HttpGet("con-progreso")]
    public async Task<IActionResult> ObtenerConProgreso()
    {
        int usuarioId = User.ObtenerUsuarioId();
        IEnumerable<ObjetivoConProgresoDto> resultado = await _objetivoService.EvaluarTodos(usuarioId);
        return Ok(resultado);
    }

    /// <summary>
    /// Devuelve el progreso de un objetivo concreto
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    [HttpGet("{id:int}/progreso")]
    public async Task<IActionResult> ObtenerProgreso(int id)
    {
        int usuarioId = User.ObtenerUsuarioId();
        Objetivo objetivo = await _objetivoRepository.ObtenerPorID(id)
            ?? throw new KeyNotFoundException("Objetivo no encontrado");

        if (objetivo.UsuarioId != usuarioId)
            return Forbid();

        ObjetivoConProgresoDto resultado = await _objetivoService.EvaluarObjetivo(id, usuarioId);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtenemos los tipos de objetivo disponibles
    /// </summary>
    /// <returns></returns>
    [HttpGet("tipos")]
    public async Task<IActionResult> ObtenerTipos()
    {
        TipoObjetivoDto[] tipos = new[]
        {
            new TipoObjetivoDto
            {
                Id = 1,
                Nombre = "Ahorro",
                Descripcion = "Controla que tu balance supere una cantidad determinada"
            },
            new TipoObjetivoDto
            {
                Id = 2,
                Nombre = "Límite de gasto",
                Descripcion = "Alerta si superas un límite de gasto"
            },
            new TipoObjetivoDto
            {
                Id = 3,
                Nombre = "Meta de ingreso",
                Descripcion = "Comprueba que alcanzas un objetivo de ingresos"
            },
            new TipoObjetivoDto
            {
                Id = 4,
                Nombre = "Reducción de deuda",
                Descripcion = "Seguimiento de la reducción del gasto"
            }
        };

        return Ok(tipos);
    }
}