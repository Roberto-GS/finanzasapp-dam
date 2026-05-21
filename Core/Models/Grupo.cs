namespace FinanzasApp.Core.Models;

public class Grupo
{
    #region CAMPOS PRIVADOS
    private int _id;
    private string _nombre = string.Empty;
    private int _creadorId;
    private DateTime _fechaCreacion = DateTime.UtcNow;
    private readonly List<UsuarioGrupo> _miembros = [];
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
            ? throw new ArgumentException("El nombre del grupo no puede estar vacío")
            : value.Trim();
    }

    public int CreadorId
    {
        get => _creadorId;
        set => _creadorId = value > 0
            ? value
            : throw new ArgumentException("El CreadorId debe ser mayor que 0");
    }

    public DateTime FechaCreacion
    {
        get => _fechaCreacion;
        private set => _fechaCreacion = value;
    }

    public Usuario? Creador { get; private set; }
    public IReadOnlyCollection<UsuarioGrupo> Miembros => _miembros.AsReadOnly();
    #endregion
}