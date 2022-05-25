
using ExchangeService.BusinessLogic.BusinessLogic.CommonPatterns;
using ExchangeService.BusinessLogic.BusinessLogic.Interfaces.Services;
using ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
using ExchangeService.DataAccessLayer;
using ExchangeService.DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddScoped<ICacheService, CacheService>();
services.AddScoped<IApiService, ApiService>();
services.AddScoped<IHistoryService, HistoryService>();
services.AddScoped<IExchangeHistoryRepository, ExchangeHistoryRepository>();
services.AddScoped<IRedirectService,ServiceMediator>(mediator => 
new ServiceMediator(mediator.GetRequiredService<ICacheService>(),
    mediator.GetRequiredService<IApiService>(),
    mediator.GetRequiredService<IHistoryService>())) ;

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

app.Run();
