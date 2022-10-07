using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Template.AuthenticationAPI.Interfaces;
using Template.AuthenticationAPI.Services;
using Template.Data.Infrastructure.Common;
using Template.Shared;

namespace Template.AuthenticationAPI.Extensions;

public static class ConfigureServicesExtensions
{
    public static void AddCustomAntiforgery(this IServiceCollection services)
    {
        services.AddAntiforgery(x => x.HeaderName = "X-XSRF-TOKEN");
        services.AddMvc(options => { options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); });
    }
    
    public static void AddCustomAuthentication(this IServiceCollection services)
    {
        // Only needed for custom roles.
        services.AddAuthorization(options =>
        {
            options.AddPolicy(CustomRoles.Admin, policy => policy.RequireRole(CustomRoles.Admin));
            options.AddPolicy(CustomRoles.User, policy => policy.RequireRole(CustomRoles.User));
            options.AddPolicy(CustomRoles.Management, policy => policy.RequireRole(CustomRoles.Management));
        });

        services.AddAuthentication(options =>
            {
                options.AddJwtAuthenticationOptions();
            })
            .AddJwtBearer(bearerConfig =>
            {
                bearerConfig.AddCommonJwtOptions();         // <-- Shared settings between AuthAPI and Gateway
                bearerConfig.Events = new JwtBearerEvents   // <-- Needed for AuthAPI to handle custom events
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
        services.AddSingleton<ISecurityService, SecurityService>();

        services.AddScoped<IAntiForgeryCookieService, AntiForgeryCookieService>();
        services.AddScoped<ITokenValidatorService, TokenValidatorService>();
        services.AddScoped<ITokenFactoryService, TokenFactoryService>();
        
        services.AddTransient<IUsersService, UsersService>();
        services.AddTransient<IRolesService, RolesService>();
        services.AddTransient<ITokenStoreService, TokenStoreService>();
    }

    public static void AddCustomExceptionHandler(this IApplicationBuilder builder)
    {
        builder.UseExceptionHandler(appBuilder =>
        {
            appBuilder.Use(async (context, next) =>
            {
                var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;
                if (error?.Error is SecurityTokenExpiredException)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        State = 401,
                        Msg = "token expired"
                    }));
                }
                else if (error?.Error != null)
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        State = 500,
                        Msg = error.Error.Message
                    }));
                }
                else
                {
                    await next();
                }
            });
        });
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