using System.ComponentModel.DataAnnotations;
using RepoDb.Attributes;

namespace Template.Data.Infrastructure.Entities;

[Map("Roles")]
public class Role
{
    [Key]
    public int Id { get; set; }
    
    public string RoleName { get; set; }
}