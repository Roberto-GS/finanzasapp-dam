using FinanzasApp.Core.Enums;

namespace FinanzasApp.Core.Models;

public class Movimiento
{
    #region CAMPOS PRIVADOS
    private int _id;
    private int _usuarioId;
    private TipoMovimiento _tipo;
    private decimal _cantidad;
    private DateTime _fecha;
    private int? _categoriaId;
    private string _nombre = string.Empty;
    private string? _descripcion;
    private string? _etiqueta;
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

    public TipoMovimiento Tipo
    {
        get => _tipo;
        set => _tipo = value;
    }

    public decimal Cantidad
    {
        get => _cantidad;
        set => _cantidad = value > 0
            ? value
            : throw new ArgumentException("La cantidad debe ser mayor que 0");
    }

    public DateTime Fecha
    {
        get => _fecha;
        set => _fecha = value > DateTime.UtcNow
            ? throw new ArgumentException("La fecha no puede ser futura")
            : value;
    }

    public int? CategoriaId
    {
        get => _categoriaId;
        set => _categoriaId = value;
    }

    public string Nombre
    {
        get => _nombre;
        set => _nombre = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("El nombre del movimiento no puede estar vacío")
            : value.Trim();
    }

    public string? Descripcion
    {
        get => _descripcion;
        set => _descripcion = value?.Trim();
    }

    public string? Etiqueta
    {
        get => _etiqueta;
        set => _etiqueta = value?.Trim();
    }

    public Usuario? Usuario { get; private set; }
    public Categoria? Categoria { get; private set; }
    #endregion
}