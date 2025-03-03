using Application.Services.CreateNewOrderService;
using Messaging.Producer;
using Microsoft.Extensions.Logging;

namespace Messaging.Services;

public class CreateNewOrderService : ICreateNewOrderService
{
    private readonly ILogger<CreateNewOrderService> _logger;
    private readonly IKafkaProducer<CreateNewOrder> _kafkaProducer;

    public CreateNewOrderService(
        ILogger<CreateNewOrderService> logger,
        IKafkaProducer<CreateNewOrder> kafkaProducer
    )
    {
        _logger = logger;
        _kafkaProducer = kafkaProducer;
    }

    public async Task CreateNewOrderAsync(CreateNewOrder createNewOrder, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting to create a new order. OrderId: {OrderId}", createNewOrder.OrderId);

            var sent = await _kafkaProducer.ProduceAsync(
                new Envelop<CreateNewOrder>(
                    topic: Topics.CarReserved,
                    key: createNewOrder.OrderId.ToString(),
                    value: createNewOrder,
                    headers: new Dictionary<string, byte[]>()
                ), 
                cancellationToken
            );

            if (!sent)
                throw new Exception($"Failed to create a new order. OrderId: {createNewOrder.OrderId}");

            _logger.LogInformation("Successfully created the order for Customer: {CustomerName}", createNewOrder.CustomerDocument);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Order creation operation was canceled. OrderId: {OrderId}", createNewOrder.OrderId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the order. OrderId: {OrderId}", createNewOrder.OrderId);
            throw;
        }
    }

}