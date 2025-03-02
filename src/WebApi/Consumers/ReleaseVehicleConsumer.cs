using Application.CommandHandlers.ReleseCar;
using Confluent.Kafka;
using MediatR;
using Messaging;
using Messaging.Consumer;
using Microsoft.Extensions.Options;

namespace Internet.Motors.VehicleCatalog.Consumers;

public class ReleaseVehicleConsumer : KafkaConsumer<ReleaseVehicleCommand>
{

    private readonly ILogger<KafkaConsumer<ReleaseVehicleCommand>> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ReleaseVehicleConsumer(
        IOptions<ConsumerConfig> consumerConfig, 
        ILogger<KafkaConsumer<ReleaseVehicleCommand>> logger,
        IServiceProvider serviceProvider
    )
        : base(consumerConfig, logger)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override string GetTopic() => "order-canceled";

    protected override async Task HandleAsync(Envelop<ReleaseVehicleCommand> envelop, CancellationToken cancellationToken)
    {
        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();
                
                var output = await mediatr.Send(envelop, cancellationToken);
                _logger.LogInformation("response from use case {@Output}", output);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An exception occurred during handling of events.");
        }
    }

}