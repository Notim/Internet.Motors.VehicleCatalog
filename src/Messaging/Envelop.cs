namespace Messaging;

public class Envelop<T>
{

    public Envelop(string topic, string key, T value, IDictionary<string, byte[]>? headers = null)
    {
        Topic = topic;
        Key = key;
        Value = value;
        Headers = headers ?? new Dictionary<string, byte[]>();
    }

    public string Topic { get; set; }

    public string? Key { get; set; }

    public IDictionary<string, byte[]> Headers { get; }

    public T? Value { get; set; }

}