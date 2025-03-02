using System.Globalization;
using Application.CommandHandlers.RegisterVehicle;
using Application.Services.CreateNewOrderService;
using Confluent.Kafka;
using Data.Cache;
using Data.Configs;
using Data.Dao;
using Data.Repositories;
using Domain;
using FluentValidation;
using MediatR;
using Messaging.Producer;
using Messaging.Services;
using Microsoft.AspNetCore.Localization;
using Serilog;
using StackExchange.Redis;

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

        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = builder.Configuration.GetConnectionString("RedisConnection");
            
            return ConnectionMultiplexer.Connect(configuration!);
        });
        
        builder.Services.AddScoped<IVehicleDao, VehicleDao>();
        builder.Services.AddScoped<IVehicleCache, VehicleCache>();
        builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();

        builder.Services.AddMediatR(typeof(RegisterVehicleCommandHandler).Assembly);
        builder.Services.AddScoped<IValidator<RegisterVehicleCommand>, RegisterVehicleCommandValidation>();

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