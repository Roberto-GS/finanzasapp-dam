namespace FinanzasApp.Core.Models;

public class Notificacion
{
    #region CAMPOS PRIVADOS
    private int _id;
    private int _usuarioId;
    private string _titulo = string.Empty;
    private string _mensaje = string.Empty;
    private string _tipo = string.Empty;
    private string _icono = string.Empty;
    private bool _leida;
    private DateTime _fechaCreacion = DateTime.UtcNow;
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
        set => _usuarioId = value;
    }

    public int? ObjetivoId { get; set; }

    public string Titulo
    {
        get => _titulo;
        set => _titulo = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("El título no puede estar vacío")
            : value.Trim();
    }

    public string Mensaje
    {
        get => _mensaje;
        set => _mensaje = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("El mensaje no puede estar vacío")
            : value.Trim();
    }

    public string Tipo
    {
        get => _tipo;
        set => _tipo = value;
    }

    public string Icono
    {
        get => _icono;
        set => _icono = value;
    }

    // Si está leída no se muestra al usuario
    public bool Leida
    {
        get => _leida;
        set => _leida = value;
    }

    public DateTime FechaCreacion
    {
        get => _fechaCreacion;
        private set => _fechaCreacion = value;
    }

    public Usuario? Usuario { get; private set; }
    public Objetivo? Objetivo { get; private set; }
    #endregion
}