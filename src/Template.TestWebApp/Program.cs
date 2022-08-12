using System.Reflection;
using Template.Data.Infrastructure.Extensions;
using Template.Data.Infrastructure.Repositories.Integration;
using Template.TestWebApp.Manager;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables()
    .AddUserSecrets(Assembly.GetExecutingAssembly());

//Add Db connection string from Secrets
builder.Services.Configure<DbSettings>
    (builder.Configuration.GetSection("DbSettings"));

builder.Services.AddControllersWithViews();

// Add services to the container.
builder.Services.AddDataInfrastructure();
builder.Services.AddTransient<IDataManager, DataManager>();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();