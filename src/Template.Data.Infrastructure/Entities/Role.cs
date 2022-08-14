using System.ComponentModel.DataAnnotations;
using RepoDb.Attributes;

namespace Template.Data.Infrastructure.Entities;

[Map("roles")]
public class Role
{
    [Key]
    public int Id { get; set; }
    
    [Map("rolename")]
    public string RoleName { get; set; }
}