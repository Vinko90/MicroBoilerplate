using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Template.Shared.Models;

public static class BearerTokensOptions
{
    public static string Key = "79f308254fabc741ee834059ae4b5b694fa978db375df692839ed21bef121d16";
    public static string Issuer = "TestCompany";
    public static string Audience = "Any";
    public static int AccessTokenExpirationMinutes = 2;
    public static int RefreshTokenExpirationMinutes = 60;
    public static bool AllowMultipleLoginsFromTheSameUser = false;
    public static bool AllowSignoutAllUserActiveClients = true;

    public static readonly TokenValidationParameters CompanyValidationParameters = new()
    {
        ValidIssuer = Issuer,           // site that makes the token
        ValidAudience = Audience,       // site that consumes the token
        RequireExpirationTime = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),
        ValidateIssuerSigningKey = true, // verify signature to avoid tampering
        ValidateLifetime = true,         // validate the expiration
        ClockSkew = TimeSpan.Zero        // tolerance for the expiration date
    };
}