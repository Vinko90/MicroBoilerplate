using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Template.AuthenticationAPI.Common;
using Template.Data.Infrastructure.Common;
using Template.Data.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);
// add services to DI container
{
    builder.Configuration.AddEnvironmentVariables()
        .AddUserSecrets(Assembly.GetExecutingAssembly());
    
    var services = builder.Services;
    
    //Add Db connection string from Secrets
    builder.Services.Configure<DbSettings>
        (builder.Configuration.GetSection("DbSettings"));

    services.AddDataInfrastructure();
    
    services.AddCustomSwagger();
    services.AddCustomServices();
    services.AddCustomJwtBearer();
    services.AddCustomCors();
    services.AddCustomAntiforgery();
}

var app = builder.Build();
// configure App
{
    app.UseExceptionHandler(appBuilder =>
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
    
    app.UseStatusCodePages();
    app.UseCustomSwagger();
    app.UseRouting();
    app.UseAuthentication();
    app.UseCors("CorsPolicy");
    app.UseAuthorization();
    
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            "default",
            "{controller=Home}/{action=Index}/{id?}");
    });
}

app.Run();