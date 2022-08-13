using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Template.AuthenticationAPI.Interfaces;
using Template.AuthenticationAPI.Services;
using Template.Data.Infrastructure.Common;

namespace Template.AuthenticationAPI.Common;

public static class ConfigureServicesExtensions
{
    public static void AddCustomAntiforgery(this IServiceCollection services)
    {
        services.AddAntiforgery(x => x.HeaderName = "X-XSRF-TOKEN");
        services.AddMvc(options => { options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); });
    }

    public static void AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                    .WithOrigins(
                        "http://localhost:4000") //Note:  The URL must be specified without a trailing slash (/).
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(host => true)
                    .AllowCredentials());
        });
    }

    public static void AddCustomJwtBearer(this IServiceCollection services)
    {
        // Only needed for custom roles.
        services.AddAuthorization(options =>
        {
            options.AddPolicy(CustomRoles.Admin, policy => policy.RequireRole(CustomRoles.Admin));
            options.AddPolicy(CustomRoles.User, policy => policy.RequireRole(CustomRoles.User));
            options.AddPolicy(CustomRoles.Management, policy => policy.RequireRole(CustomRoles.Management));
        });

        // Needed for jwt auth.
        services
            .AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = BearerTokensOptions.Issuer, // site that makes the token
                    ValidateIssuer = true,
                    ValidAudience = BearerTokensOptions.Audience, // site that consumes the token
                    ValidateAudience = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(BearerTokensOptions.Key)),
                    ValidateIssuerSigningKey = true, // verify signature to avoid tampering
                    ValidateLifetime = true, // validate the expiration
                    ClockSkew = TimeSpan.Zero // tolerance for the expiration date
                };
                cfg.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                            .CreateLogger(nameof(JwtBearerEvents));
                        logger.LogError("Authentication failed {ContextException}", context.Exception);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var tokenValidatorService =
                            context.HttpContext.RequestServices.GetRequiredService<ITokenValidatorService>();
                        return tokenValidatorService.ValidateAsync(context);
                    },
                    OnMessageReceived = context => Task.CompletedTask,
                    OnChallenge = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                            .CreateLogger(nameof(JwtBearerEvents));
                        logger.LogError("OnChallenge error {ContextError}, {ContextErrorDescription}", context.Error, context.ErrorDescription);
                        return Task.CompletedTask;
                    }
                };
            });
    }
    
    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAntiForgeryCookieService, AntiForgeryCookieService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IRolesService, RolesService>();
        services.AddSingleton<ISecurityService, SecurityService>();
        services.AddScoped<ITokenStoreService, TokenStoreService>();
        services.AddScoped<ITokenValidatorService, TokenValidatorService>();
        services.AddScoped<ITokenFactoryService, TokenFactoryService>();
    }
    
    public static void UseCustomSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(setupAction =>
        {
            setupAction.SwaggerEndpoint(
                "/swagger/AuthenticationModuleSpecification/swagger.json",
                "AuthenticationAPI");
        });
    }

    public static void AddCustomSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(setupAction =>
        {
            setupAction.SwaggerDoc(
                "AuthenticationModuleSpecification",
                new OpenApiInfo
                {
                    Title = "AuthenticationAPI",
                    Version = "1",
                    Description = "Test Environment"
                });

            setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Description = "Type JWT Bearer token below:",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new List<string>()
                }
            });
        });
    }
}