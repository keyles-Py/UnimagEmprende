using EventManager.Domain.Entities;
using EventManager.Domain.Enums;

namespace EventManager.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);
    Task<Role?> GetRoleByTypeAsync(RoleType roleType, CancellationToken cancellationToken = default);
}
