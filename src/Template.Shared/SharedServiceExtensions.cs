using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Template.Shared.Models;

namespace Template.Shared;

public static class SharedServiceExtensions
{
    public static void AddJwtAuthenticationOptions(this AuthenticationOptions options)
    {
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    }

    public static void AddCommonJwtOptions(this JwtBearerOptions cfg)
    {
        cfg.RequireHttpsMetadata = false;
        cfg.SaveToken = true;
        cfg.TokenValidationParameters = BearerTokensOptions.CompanyValidationParameters;
    }
    
    public static void AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.AllowAnyOrigin();
                    policy.AllowAnyMethod();
                    policy.AllowAnyHeader();
                    policy.SetIsOriginAllowed(_ => true);
                });
        });
    }
}