
using DataBaseLayer;
using Microsoft.EntityFrameworkCore;
using DataBaseLayer.CRUD;
using IntermediateLayer.BussinesLogic.RequestProcess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddRazorPages();
services.AddScoped<CachedInformator>();
services.AddScoped<Informator>();
services.AddScoped<Converter>();
services.AddScoped<BasicOperation>();

var configuration = builder.Configuration;

services.AddDbContext<Context>(optionsBuilder => optionsBuilder.UseSqlServer(configuration["ConnectionString"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
