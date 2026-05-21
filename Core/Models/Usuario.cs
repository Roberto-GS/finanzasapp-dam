namespace FinanzasApp.Core.Models;

public class Usuario
{
    #region CAMPOS PRIVADOS
    private int _id;
    private string _nombre = string.Empty;
    private string _email = string.Empty;
    private string _passwordHash = string.Empty;
    private DateTime _fechaCreacion = DateTime.UtcNow;
    private readonly List<Movimiento> _movimientos = [];
    private readonly List<Categoria> _categorias = [];
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
            ? throw new ArgumentException("El nombre no puede estar vacío")
            : value.Trim();
    }

    public string Email
    {
        get => _email;
        set => _email = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("El email no puede estar vacío")
            : value.Trim().ToLowerInvariant();
    }

    public string PasswordHash
    {
        get => _passwordHash;
        set => _passwordHash = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("El hash de contraseña no puede estar vacío")
            : value;
    }

    public DateTime FechaCreacion
    {
        get => _fechaCreacion;
        private set => _fechaCreacion = value;
    }

    public IReadOnlyCollection<Movimiento> Movimientos => _movimientos.AsReadOnly();
    public IReadOnlyCollection<Categoria> Categorias => _categorias.AsReadOnly();
    public IReadOnlyCollection<Objetivo> Objetivos => _objetivos.AsReadOnly();
    #endregion
}