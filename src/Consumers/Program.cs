using System.Globalization;
using Application.CommandHandlers.RegisterVehicle;
using Application.CommandHandlers.ReleseCar;
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
using Presentation.Consumers.HostedServices;
using Serilog;
using StackExchange.Redis;

namespace Presentation.Consumers;

internal class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });

        builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection(nameof(DatabaseConfig)));
        
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = builder.Configuration.GetConnectionString("RedisConnection");
            
            return ConnectionMultiplexer.Connect(configuration);
        });
        
        builder.Services.AddScoped<IVehicleDao, VehicleDao>();
        builder.Services.AddScoped<IVehicleCache, VehicleCache>();
        builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
        
        builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
        builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));
        builder.Services.AddScoped<IKafkaProducer<CreateNewOrder>, MessageProducer<CreateNewOrder>>();
        builder.Services.AddScoped<ICreateNewOrderService, CreateNewOrderService>();

        builder.Services.AddMediatR(typeof(ReleaseVehicleCommandHandler).Assembly);
        builder.Services.AddScoped<IValidator<RegisterVehicleCommand>, RegisterVehicleCommandValidation>();
        
        builder.Services.AddHostedService<ReleaseVehicleConsumer>();
        builder.Services.AddHostedService<SoldVehicleConsumer>();

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

        app.Run();
    }

}