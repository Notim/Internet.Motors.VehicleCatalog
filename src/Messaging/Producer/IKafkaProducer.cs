namespace Messaging.Producer;

public interface IKafkaProducer<T>
{

    Task<bool> ProduceAsync(Envelop<T> messageEnvelop, CancellationToken cancellationToken = default);

}