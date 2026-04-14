namespace EventManager.Application.Interfaces;

/// <summary>
/// Abstracción de hashing de contraseñas. Permite que AuthService
/// no dependa de la implementación concreta de Infrastructure.
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
