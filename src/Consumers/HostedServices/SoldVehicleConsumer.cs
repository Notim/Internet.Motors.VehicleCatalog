using Application.CommandHandlers.SoldCar;
using Confluent.Kafka;
using MediatR;
using Messaging;
using Messaging.Consumer;
using Microsoft.Extensions.Options;

namespace Presentation.Consumers.HostedServices;

public class SoldVehicleConsumer : KafkaConsumer<SoldVehicleCommand>
{

    private readonly ILogger<KafkaConsumer<SoldVehicleCommand>> _logger;
    private readonly IServiceProvider _serviceProvider;

    public SoldVehicleConsumer(
        IOptions<ConsumerConfig> consumerConfig, 
        ILogger<KafkaConsumer<SoldVehicleCommand>> logger,
        IServiceProvider serviceProvider
    )
        : base(consumerConfig, logger)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override string GetTopic() => Topics.OrderFinalizedTopic;

    protected override async Task HandleAsync(Envelop<SoldVehicleCommand> envelop, CancellationToken cancellationToken)
    {
        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();
                
                var output = await mediatr.Send(envelop.Value!, cancellationToken);
                _logger.LogInformation("response from use case {@Output} {@Errors}", output.Messages, output.FaultMessages);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An exception occurred during handling of events.");
        }
    }

}