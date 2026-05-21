namespace FinanzasApp.Core.Models;

public class TipoObjetivo
{
    #region CAMPOS PRIVADOS
    private int _id;
    private string _nombre = string.Empty;
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
            ? throw new ArgumentException("El nombre del tipo de objetivo no puede estar vacío")
            : value.Trim();
    }

    public IReadOnlyCollection<Objetivo> Objetivos => _objetivos.AsReadOnly();
    #endregion
}