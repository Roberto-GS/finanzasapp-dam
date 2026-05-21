using AutoMapper;
using FinanzasApp.Core.DTOs.Categoria;
using FinanzasApp.API.Extensions;
using FinanzasApp.Core.Interfaces;
using FinanzasApp.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanzasApp.API.Controllers;

/// <summary>
/// Gestiona las operaciones relacionadas con las categorías
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IMapper _mapper;

    public CategoriasController(ICategoriaRepository categoriaRepository, IMapper mapper)
    {
        _categoriaRepository = categoriaRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtenemos todas las categorías disponibles para el usuario autenticado
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        int usuarioId = User.ObtenerUsuarioId();
        IEnumerable<Categoria> categorias = await _categoriaRepository.ObtenerPorIdUsuario(usuarioId);
        return Ok(_mapper.Map<IEnumerable<CategoriaDto>>(categorias));
    }

    /// <summary>
    /// Obtenemos una categoría por su id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        int usuarioId = User.ObtenerUsuarioId();
        Categoria categoria = await _categoriaRepository.ObtenerPorID(id)
            ?? throw new KeyNotFoundException("Categoría no encontrada");

        if (categoria.UsuarioId != usuarioId && categoria.UsuarioId != null)
            return Forbid();

        return Ok(_mapper.Map<CategoriaDto>(categoria));
    }

    /// <summary>
    /// Creamos una nueva categoría para el usuario
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearCategoriaDto dto)
    {
        int usuarioId = User.ObtenerUsuarioId();
        Categoria categoria = _mapper.Map<Categoria>(dto);
        categoria.UsuarioId = usuarioId;

        await _categoriaRepository.Agregar(categoria);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = categoria.Id },
            _mapper.Map<CategoriaDto>(categoria));
    }

    /// <summary>
    /// Actualizamos una categoría existente
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, [FromBody] CrearCategoriaDto dto)
    {
        int usuarioId = User.ObtenerUsuarioId();
        Categoria categoria = await _categoriaRepository.ObtenerPorID(id)
            ?? throw new KeyNotFoundException("Categoría no encontrada");

        if (categoria.UsuarioId != usuarioId)
            return Forbid();

        categoria.Nombre = dto.Nombre;
        categoria.Color = dto.Color;

        await _categoriaRepository.Actualizar(categoria);
        return Ok(_mapper.Map<CategoriaDto>(categoria));
    }

    /// <summary>
    /// Eliminamos una categoría existente, los movimientos asociados pasan a estar sin categoría
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        int usuarioId = User.ObtenerUsuarioId();
        Categoria categoria = await _categoriaRepository.ObtenerPorID(id)
            ?? throw new KeyNotFoundException("Categoría no encontrada");

        if (categoria.UsuarioId != usuarioId)
            return Forbid();

        await _categoriaRepository.Eliminar(id);
        return NoContent();
    }
}