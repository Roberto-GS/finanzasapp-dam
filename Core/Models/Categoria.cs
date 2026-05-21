namespace FinanzasApp.Core.Models;

public class Categoria
{
    #region CAMPOS PRIVADOS
    private int _id;
    private string _nombre = string.Empty;
    private string? _color;
    private int? _usuarioId;
    private readonly List<Movimiento> _movimientos = [];
    private readonly List<Objetivo> _objetivos = [];
    #endregion

    #region PROPIEDADES
    public int Id
    {
        get => _id;
        private set => _id = value;
    }

    public string Nombre
    {
        get => _nombre;
        set => _nombre = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("El nombre de la categoría no puede estar vacío.")
            : value.Trim();
    }

    public string? Color
    {
        get => _color;
        set => _color = value?.Trim();
    }

    public int? UsuarioId
    {
        get => _usuarioId;
        set => _usuarioId = value;
    }

    public Usuario? Usuario { get; private set; }

    public IReadOnlyCollection<Movimiento> Movimientos => _movimientos.AsReadOnly();
    public IReadOnlyCollection<Objetivo> Objetivos => _objetivos.AsReadOnly();
    #endregion
}