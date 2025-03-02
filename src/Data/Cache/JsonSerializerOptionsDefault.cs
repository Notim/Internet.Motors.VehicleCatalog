using System.Text.Json;
using System.Text.Json.Serialization;

namespace Data.Cache;

public static class JsonSerializerOptionsDefault
{

    public static JsonSerializerOptions Default =>
        new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

}