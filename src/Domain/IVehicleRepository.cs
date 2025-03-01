namespace Domain
{

    public interface IVehicleRepository
    {

        Task<Vehicle?> GetVehicleByVehicleId(Guid vehicleId);

        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();

        Task<int> InsertVehicleAsync(Vehicle vehicle);

        Task<bool> UpdateVehicleAsync(Vehicle vehicle);

    }

}