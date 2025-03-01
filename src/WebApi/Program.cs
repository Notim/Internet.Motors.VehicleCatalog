using System.Globalization;
using Application.CommandHandlers;
using Application.CommandHandlers.RegisterVehicle;
using Data.Configs;
using Data.Repositories;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Internet.Motors.VehicleCatalog;

internal class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();
        
        builder.Services.Configure<DatabaseConfig>(
            builder.Configuration.GetSection(nameof(DatabaseConfig))
        );

        builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
        
        builder.Services.AddMediatR(typeof(RegisterVehicleCommandHandler).Assembly);

        var app = builder.Build();
        
        var defaultCulture = "en-US";
        
        var supportedCultures = new[]
        {
            new CultureInfo(defaultCulture),
            new CultureInfo("pt-BR")
        };
        
        var localizationOptions = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(defaultCulture),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        };
        
        app.UseRequestLocalization(localizationOptions);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapControllers();

        app.Run();
    }

}