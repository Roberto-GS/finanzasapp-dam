namespace FinanzasApp.Core.Models;

public class Rol
{
    #region CAMPOS PRIVADOS
    private int _id;
    private string _nombre = string.Empty;
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
            ? throw new ArgumentException("El nombre del rol no puede estar vacío")
            : value.Trim();
    }
    #endregion
}