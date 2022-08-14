using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("Ocelot.json");

builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddOcelot();

var app = builder.Build();

//app.UseSwagger();
//app.UseSwaggerUI();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.UseOcelot().Wait();
app.Run();