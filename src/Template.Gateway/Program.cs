using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Template.Shared;

var builder = WebApplication.CreateBuilder(args);
{
    //Add the Ocelot configuration
    //This can be different depending on the running environment
    builder.Configuration.AddJsonFile("Ocelot.json");
    
    var services = builder.Services;

    //Add common JWT validation settings
    services.AddAuthentication(options =>
        {
            options.AddJwtAuthenticationOptions();
        })
        .AddJwtBearer(bearerConfig =>
        {
            bearerConfig.AddCommonJwtOptions();
        });

    services.AddControllers();
    services.AddCustomCors();
    services.AddOcelot();
}

var app = builder.Build();
{
    app.UseRouting();
    app.UseAuthorization();
    app.MapControllers();
    app.UseCors();
}

app.UseOcelot().Wait();
app.Run();