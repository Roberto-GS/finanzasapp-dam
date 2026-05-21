using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using FinanzasApp.API.Settings;
using FinanzasApp.Core.DTOs.Autentificacion;
using FinanzasApp.Core.DTOs.Usuario;
using FinanzasApp.Core.Models;
using FinanzasApp.Core.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FinanzasApp.API.Services;

/// <summary>
/// Implementamos la lógica de autenticación, tanto para iniciar sesión como para registrar un nuevo usuario. Verificamos las credenciales, generamos el token JWT y devolvemos la información del usuario junto con el token. Utilizamos BCrypt para el hashing de contraseñas y AutoMapper para mapear entre entidades y DTOs.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly IMapper _mapper;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IOptions<JwtSettings> jwtSettings,
        IMapper mapper)
    {
        _usuarioRepository = usuarioRepository;
        _jwtSettings = jwtSettings.Value;
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task<AuthResponseDto> IniciarSesion(LoginDto dto)
    {
        // Buscamos el usuario por email y verificamos la contraseña
        Usuario usuario = await _usuarioRepository.ObtenerPorEmail(dto.Email)
            ?? throw new UnauthorizedAccessException("Credenciales incorrectas");

        bool passwordValida = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash);
        if (!passwordValida)
            throw new UnauthorizedAccessException("Credenciales incorrectas");

        string token = GenerarTokenJwt(usuario);

        return new AuthResponseDto
        {
            Token = token,
            Usuario = _mapper.Map<UsuarioDto>(usuario)
        };
    }

    /// <inheritdoc/>
    public async Task<AuthResponseDto> Registrar(RegistroDto dto)
    {
        // Verificamos que no existe ya una cuenta con ese email
        Usuario? existe = await _usuarioRepository.ObtenerPorEmail(dto.Email);
        if (existe is not null)
            throw new ArgumentException("Ya existe una cuenta con ese email");

        Usuario usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await _usuarioRepository.Añadir(usuario);

        string token = GenerarTokenJwt(usuario);

        return new AuthResponseDto
        {
            Token = token,
            Usuario = _mapper.Map<UsuarioDto>(usuario)
        };
    }

    /// <summary>
    /// Generamos un token JWT con la información del usuario, utilizando la configuración definida en JwtSettings para el secreto, el emisor, el público y la duración del token. Incluimos claims con el ID, email y nombre del usuario para que puedan ser utilizados en la autorización de rutas protegidas.
    /// </summary>
    /// <param name="usuario"></param>
    /// <returns></returns>
    private string GenerarTokenJwt(Usuario usuario)
    {
        SymmetricSecurityKey clave = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        SigningCredentials credenciales = new SigningCredentials(
            clave, SecurityAlgorithms.HmacSha256);

        Claim[] claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours),
            signingCredentials: credenciales
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}