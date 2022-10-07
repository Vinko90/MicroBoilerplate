using Template.AuthenticationAPI.Extensions;
using Template.Data.Infrastructure.Common;
using Template.Data.Infrastructure.Extensions;
using Template.Shared;

var builder = WebApplication.CreateBuilder(args);
{
    var services = builder.Services;
    
    //Add Db connection string from Secrets
    services.Configure<DbSettings>
        (builder.Configuration.GetSection("DbSettings"));

    services.AddDataInfrastructure();
    
    services.AddCustomSwagger();
    services.AddCustomServices();
    services.AddCustomAuthentication();
    services.AddCustomCors();
    services.AddCustomAntiforgery();
}

var app = builder.Build();
{
    app.AddCustomExceptionHandler();
    app.UseStatusCodePages();
    app.UseCustomSwagger();
    app.UseRouting();
    app.UseAuthentication();
    app.UseCors();
    app.UseAuthorization();
    
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            "default",
            "{controller=Home}/{action=Index}/{id?}");
    });
}

app.Run();