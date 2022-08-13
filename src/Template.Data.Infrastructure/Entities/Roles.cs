using System.ComponentModel.DataAnnotations;

namespace Template.Data.Infrastructure.Entities;

public class Roles
{
    [Key]
    public int Id { get; set; }
    
    public string RoleName { get; set; }
}