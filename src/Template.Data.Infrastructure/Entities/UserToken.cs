using System.ComponentModel.DataAnnotations;
using RepoDb.Attributes;

namespace Template.Data.Infrastructure.Entities;

[Map("UserTokens")]
public class UserToken
{
    [Key]
    public int Id { get; set; }

    public string? AccessTokenHash { get; set; }

    public DateTimeOffset AccessTokenExpiresDateTime { get; set; }

    public string RefreshTokenIdHash { get; set; }

    public string? RefreshTokenIdHashSource { get; set; }

    public DateTimeOffset RefreshTokenExpiresDateTime { get; set; }

    public int UserId { get; set; }
}