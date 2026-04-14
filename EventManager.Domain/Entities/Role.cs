using EventManager.Domain.Common;
using EventManager.Domain.Enums;

namespace EventManager.Domain.Entities;

public class Role : BaseEntity
{
    public RoleType Name { get; set; }
    public string Description { get; set; } = string.Empty;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
