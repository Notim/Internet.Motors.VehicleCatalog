using Application.CommandHandlers;
using Data.Configs;
using Data.Repositories;
using Domain;
using MediatR;

namespace Internet.Motors.VehicleCatalog;

internal class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.Configure<DatabaseConfig>(
            builder.Configuration.GetSection(nameof(DatabaseConfig))
        );

        builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
        
        builder.Services.AddMediatR(typeof(RegisterVehicleCommandHandler).Assembly);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.Run();
    }

}