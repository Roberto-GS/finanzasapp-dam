namespace FinanzasApp.Core.Models;

public class SolicitudGrupo
{
    #region CAMPOS PRIVADOS
    private int _id;
    private int _grupoId;
    private int _solicitanteId;
    private int _invitadoId;
    private string _estado = "pendiente";
    private DateTime _fechaSolicitud = DateTime.UtcNow;
    #endregion

    #region PROPIEDADES
    public int Id
    {
        get => _id;
        private set => _id = value;
    }

    public int GrupoId
    {
        get => _grupoId;
        set => _grupoId = value;
    }

    public int SolicitanteId
    {
        get => _solicitanteId;
        set => _solicitanteId = value;
    }

    public int InvitadoId
    {
        get => _invitadoId;
        set => _invitadoId = value;
    }

    public string Estado
    {
        get => _estado;
        set => _estado = value is "pendiente" or "aceptada" or "rechazada"
            ? value
            : throw new ArgumentException("Estado no válido");
    }

    public DateTime FechaSolicitud
    {
        get => _fechaSolicitud;
        private set => _fechaSolicitud = value;
    }

    public Grupo? Grupo { get; private set; }
    public Usuario? Solicitante { get; private set; }
    public Usuario? Invitado { get; private set; }
    #endregion
}