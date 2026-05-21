using FinanzasApp.Core.Enums;

namespace FinanzasApp.Core.Models;

public class Objetivo
{
    #region CAMPOS PRIVADOS
    private int _id;
    private int _usuarioId;
    private int _tipoId;
    private int? _categoriaId;
    private decimal? _cantidadObjetivo;
    private PeriodoObjetivo _periodo;
    private string? _descripcion;
    private DateTime? _fechaInicio;
    private DateTime? _fechaFin;
    private bool _activo = true;
    #endregion

    #region PROPIEDADES
    public int Id
    {
        get => _id;
        private set => _id = value;
    }

    public int UsuarioId
    {
        get => _usuarioId;
        set => _usuarioId = value > 0
            ? value
            : throw new ArgumentException("El UsuarioId debe ser mayor que 0");
    }

    public int TipoId
    {
        get => _tipoId;
        set => _tipoId = value > 0
            ? value
            : throw new ArgumentException("El TipoId debe ser mayor que 0");
    }

    public int? CategoriaId
    {
        get => _categoriaId;
        set => _categoriaId = value;
    }

    public decimal? CantidadObjetivo
    {
        get => _cantidadObjetivo;
        set => _cantidadObjetivo = value is null || value > 0
            ? value
            : throw new ArgumentException("La cantidad objetivo debe ser mayor que 0");
    }

    public PeriodoObjetivo Periodo
    {
        get => _periodo;
        set => _periodo = value;
    }

    public string? Descripcion
    {
        get => _descripcion;
        set => _descripcion = value?.Trim();
    }

    public DateTime? FechaInicio
    {
        get => _fechaInicio;
        set => _fechaInicio = value;
    }

    public DateTime? FechaFin
    {
        get => _fechaFin;
        set
        {
            if (value.HasValue && _fechaInicio.HasValue && value < _fechaInicio)
                throw new ArgumentException("La fecha fin no puede ser anterior a la fecha de inicio");
            _fechaFin = value;
        }
    }

    public bool Activo
    {
        get => _activo;
        set => _activo = value;
    }

    public bool EstaVencido => _fechaFin.HasValue && _fechaFin.Value < DateTime.UtcNow;
    public bool ProximoAVencer(int dias) => _fechaFin.HasValue && (_fechaFin.Value - DateTime.UtcNow).TotalDays <= dias;

    public Usuario? Usuario { get; private set; }
    public TipoObjetivo? TipoObjetivo { get; private set; }
    public Categoria? Categoria { get; private set; }
    #endregion
}