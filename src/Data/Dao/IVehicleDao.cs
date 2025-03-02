using Data.Dao.Dtos;

namespace Data.Dao;

public interface IVehicleDao
{

    Task<VehicleDto?> GetVehicleByVehicleId(Guid vehicleId);

    Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync();

    Task<int> InsertVehicleAsync(VehicleDto vehicle);

    Task<bool> UpdateVehicleAsync(VehicleDto vehicle);

}