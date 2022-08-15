namespace Template.AuthenticationAPI.Common;

public static class BearerTokensOptions
{
    public static string Key = "79f308254fabc741ee834059ae4b5b694fa978db375df692839ed21bef121d16";
    public static string Issuer = "TestCompany";
    public static string Audience = "Any";
    public static int AccessTokenExpirationMinutes = 2;
    public static int RefreshTokenExpirationMinutes = 60;
    public static bool AllowMultipleLoginsFromTheSameUser = false;
    public static bool AllowSignoutAllUserActiveClients = true;
}