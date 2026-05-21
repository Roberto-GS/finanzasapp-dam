using AutoMapper;
using FinanzasApp.Core.DTOs.Autentificacion;
using FinanzasApp.Core.DTOs.Categoria;
using FinanzasApp.Core.DTOs.Movimiento;
using FinanzasApp.Core.DTOs.Objetivo;
using FinanzasApp.Core.DTOs.Usuario;
using FinanzasApp.Core.Models;

namespace FinanzasApp.Infrastructure.Mappings;

/// <summary>
/// Es el perfil del AutoMapper, definimos todas las conversiones entre los modelos de dominio y los DTOs de la aplicación
/// </summary>
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Usuario
        CreateMap<Usuario, UsuarioDto>().ReverseMap();

        // Categoria
        CreateMap<Categoria, CategoriaDto>().ReverseMap();
        CreateMap<CrearCategoriaDto, Categoria>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore());

        // Movimiento
        // Al mapear a MovimientoDto incluimos datos de la categoría
        CreateMap<Movimiento, MovimientoDto>()
            .ForMember(dest => dest.CategoriaNombre,
                opt => opt.MapFrom(src =>
                    src.Categoria != null ? src.Categoria.Nombre : null))
            .ForMember(dest => dest.CategoriaColor,
                opt => opt.MapFrom(src =>
                    src.Categoria != null ? src.Categoria.Color : null));

        // Al crear un movimiento la fecha se genera automáticamente
        CreateMap<CrearMovimientoDto, Movimiento>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Fecha, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Categoria, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore());

        // Al editar mantenemos la fecha original
        CreateMap<EditarMovimientoDto, Movimiento>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Fecha, opt => opt.Ignore())
            .ForMember(dest => dest.Categoria, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore());

        // Objetivo
        // Incluimos los nombre del tipo y la categoría desde las navegaciones
        CreateMap<Objetivo, ObjetivoDto>()
            .ForMember(dest => dest.TipoNombre,
                opt => opt.MapFrom(src =>
                    src.TipoObjetivo != null ? src.TipoObjetivo.Nombre : null))
            .ForMember(dest => dest.CategoriaNombre,
                opt => opt.MapFrom(src =>
                    src.Categoria != null ? src.Categoria.Nombre : null))
            .ForMember(dest => dest.EstaVencido,
                opt => opt.MapFrom(src => src.EstaVencido));

        // Al crear un objetivo se marca como activo por defecto
        CreateMap<CrearObjetivoDto, Objetivo>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.TipoObjetivo, opt => opt.Ignore())
            .ForMember(dest => dest.Categoria, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore());

        // TipoObjetivo
        CreateMap<TipoObjetivo, TipoObjetivoDto>().ReverseMap();
    }
}