using RepoDb.Attributes;

namespace Template.Data.Infrastructure.Entities;

[Map("userroles")]
public class UserRole
{
    [Map("userid")]
    public int UserId { get; set; }
    
    [Map("roleid")]
    public int RoleId { get; set; }
}