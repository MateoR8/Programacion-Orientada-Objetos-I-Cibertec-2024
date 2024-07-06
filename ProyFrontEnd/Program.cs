using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ProyFrontEnd.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();
IConfiguration configuration = app.Services.GetRequiredService<IConfiguration>();
MetaGlobal.LoadRutaApi(configuration["ConnectionStrings:RutaAPi"]);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
