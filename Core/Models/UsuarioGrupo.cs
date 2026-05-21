namespace FinanzasApp.Core.Models;

public class UsuarioGrupo
{
    #region CAMPOS PRIVADOS
    private int _usuarioId;
    private int _grupoId;
    private int _rolId;
    private DateTime _fechaUnion = DateTime.UtcNow;
    #endregion

    #region PROPIEDADES
    public int UsuarioId
    {
        get => _usuarioId;
        set => _usuarioId = value;
    }

    public int GrupoId
    {
        get => _grupoId;
        set => _grupoId = value;
    }

    public int RolId
    {
        get => _rolId;
        set => _rolId = value;
    }

    public DateTime FechaUnion
    {
        get => _fechaUnion;
        private set => _fechaUnion = value;
    }

    public Usuario? Usuario { get; private set; }
    public Grupo? Grupo { get; private set; }
    public Rol? Rol { get; private set; }
    #endregion
}