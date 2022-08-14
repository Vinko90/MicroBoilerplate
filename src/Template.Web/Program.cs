using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Template.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:4000/")
}); 

//builder.Services
  //  .AddTransient<CookieHandler>()
   // .AddScoped(sp => sp
    //    .GetRequiredService<IHttpClientFactory>()
     //   .CreateClient("API"))
   // .AddHttpClient("API", client => client.BaseAddress = new Uri("http://localhost:4000/"))
   // .AddHttpMessageHandler<CookieHandler>();

builder.Services.AddMudServices();

await builder.Build().RunAsync();
