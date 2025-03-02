using System.Text.Json;
using System.Text.Json.Serialization;

namespace Messaging;

public static class JsonSerializerOptionsDefault
{

    public static JsonSerializerOptions Default =>
        new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

}