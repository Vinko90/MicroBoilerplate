using System.ComponentModel.DataAnnotations;
using RepoDb.Attributes;

namespace Template.Data.Infrastructure.Entities;

[Map("users")]
public class User
{
    [Key]
    public int Id { get; set; }

    [Map("username")]
    public string Username { get; set; }

    [Map("password")]
    public string Password { get; set; }

    [Map("displayname")]
    public string? DisplayName { get; set; }

    [Map("isactive")]
    public bool IsActive { get; set; }

    [Map("lastloggedin")]
    public DateTime? LastLoggedIn { get; set; }

    /// <summary>
    ///     every time the user changes his Password,
    ///     or an admin changes his Roles or stat/IsActive,
    ///     create a new `SerialNumber` GUID and store it in the DB.
    /// </summary>
    [Map("serialnumber")]
    public string? SerialNumber { get; set; }
}