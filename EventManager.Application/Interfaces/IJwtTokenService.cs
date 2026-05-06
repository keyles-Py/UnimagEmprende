using EventManager.Domain.Entities;

namespace EventManager.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user, string role);
}
