using Data.Cache;
using Data.Dao;
using Data.Mappers;
using Domain;
using Microsoft.Extensions.Logging;

namespace Data.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly IVehicleDao _vehicleDao;
    private readonly IVehicleCache _vehicleCache;
    private readonly ILogger<VehicleRepository> _logger;

    public VehicleRepository(
        IVehicleDao vehicleDao,
        IVehicleCache vehicleCache,
        ILogger<VehicleRepository> logger)
    {
        _vehicleDao = vehicleDao ?? throw new ArgumentNullException(nameof(vehicleDao));
        _vehicleCache = vehicleCache ?? throw new ArgumentNullException(nameof(vehicleCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Vehicle?> GetVehicleByVehicleId(Guid vehicleId)
    {
        try
        {
            var cachedVehicle = await _vehicleCache.GetVehicleByVehicleId(vehicleId);
            if (cachedVehicle != null)
            {
                _logger.LogInformation("Vehicle with ID {VehicleId} retrieved from cache.", vehicleId);
                
                return cachedVehicle.MapToDomainModel();
            }
            
            var vehicleDto = await _vehicleDao.GetVehicleByVehicleId(vehicleId);
            if (vehicleDto != null)
            {
                await _vehicleCache.AddOrUpdateVehicleAsync(vehicleDto);
                _logger.LogInformation("Vehicle with ID {VehicleId} retrieved from database and updated in cache.", vehicleId);
            }

            return vehicleDto.MapToDomainModel();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving vehicle with ID {VehicleId}.", vehicleId);
            throw;
        }
    }

    public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
    {
        try
        {
            var cachedVehicles = await _vehicleCache.GetAllVehiclesAsync();
            if (cachedVehicles.Any())
            {
                _logger.LogInformation("Vehicles retrieved from cache.");
                return cachedVehicles.MapToDomainModel();
            }

            var vehicleDtos = await _vehicleDao.GetAllVehiclesAsync();

            foreach (var vehicleDto in vehicleDtos)
            {
                await _vehicleCache.AddOrUpdateVehicleAsync(vehicleDto);
            }

            _logger.LogInformation("Vehicles retrieved from database and updated in cache.");
            return vehicleDtos.MapToDomainModel();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all vehicles.");
            throw;
        }
    }

    public async Task<int> InsertVehicleAsync(Vehicle vehicle)
    {
        try
        {
            var vehicleDto = vehicle.MapToDto();

            var newId = await _vehicleDao.InsertVehicleAsync(vehicleDto);
            if (newId > 0)
            {
                vehicleDto.Id = newId;
                await _vehicleCache.AddOrUpdateVehicleAsync(vehicleDto);
                _logger.LogInformation("Vehicle with ID {VehicleId} inserted into database and cache.", vehicle.VehicleId);
            }
            
            vehicle.SetId(newId);
            
            return newId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while inserting vehicle with ID {VehicleId}.", vehicle.VehicleId);
            throw;
        }
    }

    public async Task<bool> UpdateVehicleAsync(Vehicle vehicle)
    {
        try
        {
            var vehicleDto = vehicle.MapToDto();

            var isUpdated = await _vehicleDao.UpdateVehicleAsync(vehicleDto);
            if (isUpdated)
            {
                await _vehicleCache.AddOrUpdateVehicleAsync(vehicleDto);
                _logger.LogInformation("Vehicle with ID {VehicleId} updated in database and cache.", vehicle.VehicleId);
            }

            return isUpdated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating vehicle with ID {VehicleId}.", vehicle.VehicleId);
            throw;
        }
    }
    
}