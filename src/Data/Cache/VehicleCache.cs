using System.Text.Json;
using Data.Dao.Dtos;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Data.Cache;

public class VehicleCache : IVehicleCache
{
    private readonly IDatabase _redisDatabase;
    private readonly ILogger<VehicleCache> _logger;

    private const string VehicleKeyPrefix = "Vehicle";

    public VehicleCache(
        IConnectionMultiplexer redisConnection, 
        ILogger<VehicleCache> logger
    )
    {
        _redisDatabase = redisConnection.GetDatabase();
        _logger = logger;
    }

    public async Task<VehicleDto?> GetVehicleByVehicleId(Guid vehicleId)
    {
        var redisKey = $"{VehicleKeyPrefix}:{vehicleId}";
        try
        {
            string? vehicleData = await _redisDatabase.StringGetAsync(redisKey);
            return string.IsNullOrEmpty(vehicleData) 
                       ? null 
                       : JsonSerializer.Deserialize<VehicleDto>(vehicleData, JsonSerializerOptionsDefault.Default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve vehicle with ID {VehicleId} from Redis.", vehicleId);
            throw;
        }
    }

    public async Task<IList<VehicleDto>> GetAllVehiclesAsync()
    {
        try
        {
            var server = _redisDatabase.Multiplexer.GetServer(_redisDatabase.Multiplexer.GetEndPoints()[0]);
            var keys = server.Keys(pattern: $"{VehicleKeyPrefix}:*");

            var vehicles = new List<VehicleDto>();
            foreach (var key in keys)
            {
                var value = await _redisDatabase.StringGetAsync(key);
                if (!string.IsNullOrEmpty(value))
                {
                    var vehicle = JsonSerializer.Deserialize<VehicleDto>(value, JsonSerializerOptionsDefault.Default);
                    if (vehicle != null)
                    {
                        vehicles.Add(vehicle);
                    }
                }
            }

            return vehicles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve all vehicles from Redis.");
            throw;
        }
    }

    public async Task<bool> AddOrUpdateVehicleAsync(VehicleDto? vehicle)
    {
        if (vehicle == null)
        {
            _logger.LogError("Vehicle instance cannot be null.");
            return false;
        }

        var redisKey = $"{VehicleKeyPrefix}:{vehicle.VehicleId}";
        try
        {
            var value = JsonSerializer.Serialize(vehicle, JsonSerializerOptionsDefault.Default);
            var added = await _redisDatabase.StringSetAsync(redisKey, value, TimeSpan.MaxValue);

            _logger.LogInformation("Vehicle with ID {VehicleId} was successfully added or updated in Redis.", vehicle.VehicleId);
            return added;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add or update vehicle with ID {VehicleId} in Redis.", vehicle.VehicleId);
            throw;
        }
    }
    
    public void FlushCollection()
    {
        _redisDatabase.Execute("FLUSHDB");
    }
}