using System.Security.Claims;

namespace Template.AuthenticationAPI.Common;

public class JwtTokensData
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string RefreshTokenSerial { get; set; }
    public IEnumerable<Claim> Claims { get; set; }
}