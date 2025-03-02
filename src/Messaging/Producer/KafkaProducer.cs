using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Producer;

public class MessageProducer<T> : IKafkaProducer<T>
{
    private readonly ILogger<MessageProducer<T>> _logger;
    private readonly ProducerConfig _producerConfig;

    public MessageProducer(IOptions<ProducerConfig> producerConfig, ILogger<MessageProducer<T>> logger)
    {
        _producerConfig = producerConfig.Value;
        _logger = logger;
    }

    public async Task<bool> ProduceAsync(Envelop<T> messageEnvelop, CancellationToken cancellationToken = default)
    {
        using var producer = new ProducerBuilder<string, string>(_producerConfig).Build();
        try
        {
            _logger.LogInformation("[{Source}] - Starting message production", nameof(ProduceAsync));

            var message = new Message<string?, string>
            {
                Key = messageEnvelop.Key,
                Value = JsonSerializer.Serialize(messageEnvelop.Value, JsonSerializerOptionsDefault.Default),
                Headers = new Headers()
            };

            foreach (var header in messageEnvelop.Headers)
            {
                message.Headers.Add(header.Key, header.Value);
            }

            _logger.LogDebug("[{Source}] - Message created with key: {Key}", nameof(ProduceAsync), message.Key);

            var result = await producer.ProduceAsync(messageEnvelop.Topic, message, cancellationToken);
            
            if (result.Status != PersistenceStatus.Persisted)
            {
                throw new Exception($"Message delivery failed or status is uncertain for topic: {messageEnvelop.Topic}");
            }

            _logger.LogInformation("[{Source}] - Message successfully produced to topic: {Topic}", nameof(ProduceAsync), result.Topic);

            return true;
        }
        catch (ProduceException<string, T> ex)
        {
            _logger.LogError("[{Source}] - An error occurred while producing message: {Error}", nameof(ProduceAsync), ex.Message);
            return false;
        }
    }
}