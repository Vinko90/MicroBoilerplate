using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Template.AuthenticationAPI.Interfaces;

public interface ITokenValidatorService
{
    Task ValidateAsync(TokenValidatedContext context);
}