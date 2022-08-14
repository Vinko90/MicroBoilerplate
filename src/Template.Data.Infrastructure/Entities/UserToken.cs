using System.ComponentModel.DataAnnotations;
using RepoDb.Attributes;

namespace Template.Data.Infrastructure.Entities;

[Map("usertokens")]
public class UserToken
{
    [Key]
    public int Id { get; set; }

    [Map("accesstokenhash")]
    public string? AccessTokenHash { get; set; }
    
    [Map("accesstokenexpiresdatetime")]
    public DateTime AccessTokenExpiresDateTime { get; set; }

    [Map("refreshtokenidhash")]
    public string RefreshTokenIdHash { get; set; }

    [Map("refreshtokenidhashsource")]
    public string? RefreshTokenIdHashSource { get; set; }

    [Map("refreshtokenexpiresdatetime")]
    public DateTime RefreshTokenExpiresDateTime { get; set; }

    [Map("userid")]
    public int UserId { get; set; }
}