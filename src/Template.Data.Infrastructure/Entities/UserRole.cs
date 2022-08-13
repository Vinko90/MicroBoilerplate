using RepoDb.Attributes;

namespace Template.Data.Infrastructure.Entities;

[Map("UserRoles")]
public class UserRole
{
    public int UserId { get; set; }
    
    public int RoleId { get; set; }
}