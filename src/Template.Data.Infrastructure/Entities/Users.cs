using System.ComponentModel.DataAnnotations;

namespace Template.Data.Infrastructure.Entities;

public class Users
{
    [Key]
    public int Id { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string DisplayName { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset? LastLoggedIn { get; set; }

    /// <summary>
    ///     every time the user changes his Password,
    ///     or an admin changes his Roles or stat/IsActive,
    ///     create a new `SerialNumber` GUID and store it in the DB.
    /// </summary>
    public string SerialNumber { get; set; }
}