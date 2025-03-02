using Data.Dao.Dtos;

namespace Data.Cache;

public interface IVehicleCache
{

    Task<VehicleDto?> GetVehicleByVehicleId(Guid vehicleId);

    Task<IList<VehicleDto>> GetAllVehiclesAsync();

    Task<bool> AddOrUpdateVehicleAsync(VehicleDto? vehicle);

    void FlushCollection();

}