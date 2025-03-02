using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Consumer;

public abstract class KafkaConsumer<T> : BackgroundService
{

    private readonly ConsumerConfig _consumerConfig;
    private readonly ILogger<KafkaConsumer<T>> _logger;

    protected abstract string GetTopic();

    protected KafkaConsumer(
        IOptions<ConsumerConfig> consumerConfig,
        ILogger<KafkaConsumer<T>> logger
    )
    {
        _consumerConfig = consumerConfig.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(
            async () => {
                using var consumer = new ConsumerBuilder<string, string>(_consumerConfig).Build();
                consumer.Subscribe(GetTopic());
                
                try
                {   _logger.LogInformation("Background Service is starting.");
                
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(stoppingToken);
                        
                            _logger.LogInformation("Received message: Key = {Key}, Value = {Value}", consumeResult.Message.Key, consumeResult.Message.Value);
                            
                            var envelop = new Envelop<T>(
                                topic: GetTopic(),
                                key: consumeResult.Message.Key,
                                value: JsonSerializer.Deserialize<T>(consumeResult.Message.Value, JsonSerializerOptionsDefault.Default)!,
                                headers: consumeResult.Message.Headers.ToDictionary(header => header.Key, header => header.GetValueBytes()) 
                            );

                            await HandleAsync(envelop, stoppingToken);
                        }
                        catch (ConsumeException ex)
                        {
                            _logger.LogError(ex, ex.Message);
                        }
                    
                        await Task.Delay(100, stoppingToken);
                    }
                
                    _logger.LogInformation("Background Service is stopping.");
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogInformation(ex, "Cancellation requested.");
                }
                finally
                {
                    consumer.Close();
                }
            }, 
            stoppingToken
        );
    }

    protected abstract Task HandleAsync(Envelop<T> envelop, CancellationToken cancellationToken);

}