using System.Globalization;
using Application.CommandHandlers.RegisterVehicle;
using Application.Services.CreateNewOrderService;
using Confluent.Kafka;
using Data.Configs;
using Data.Repositories;
using Domain;
using MediatR;
using Messaging.Producer;
using Messaging.Services;
using Microsoft.AspNetCore.Localization;
using Serilog;

namespace Presentation.WebApi;

internal class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });

        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();

        builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));
        builder.Services.AddScoped<IKafkaProducer<CreateNewOrder>, MessageProducer<CreateNewOrder>>();
        builder.Services.AddScoped<ICreateNewOrderService, CreateNewOrderService>();

        builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection(nameof(DatabaseConfig)));

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