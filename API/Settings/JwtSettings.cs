namespace FinanzasApp.API.Settings;

/// <summary>
/// Esta clase representa la configuración necesaria para generar y validar tokens JWT en la aplicación, incluyendo la clave secreta, el issuer, el audience y el tiempo de expiración del token. Esta configuración se utiliza en el servicio de autenticación para crear tokens seguros y confiables que permitan a los usuarios autenticarse y acceder a los recursos protegidos de la API.
/// </summary>
public class JwtSettings
{
    private string _secretKey = string.Empty;
    private string _issuer = string.Empty;
    private string _audience = string.Empty;
    private int _expirationHours = 72;

    public string SecretKey
    {
        get => _secretKey;
        set => _secretKey = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("La clave secreta JWT no puede estar vacía")
            : value;
    }

    public string Issuer
    {
        get => _issuer;
        set => _issuer = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("El issuer JWT no puede estar vacío")
            : value;
    }

    public string Audience
    {
        get => _audience;
        set => _audience = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("El audience JWT no puede estar vacío")
            : value;
    }

    public int ExpirationHours
    {
        get => _expirationHours;
        set => _expirationHours = value > 0
            ? value
            : throw new ArgumentException("Las horas de expiración deben ser mayores que 0");
    }
}