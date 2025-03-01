namespace Domain
{

    public interface IVehicleRepository
    {

        /// <summary>
        /// Retrieves all vehicles from the database.
        /// </summary>
        /// <returns>A list of vehicles.</returns>
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();

        /// <summary>
        /// Inserts a new vehicle into the database.
        /// </summary>
        /// <param name="vehicle">The vehicle to insert.</param>
        /// <returns>The ID of the newly inserted vehicle.</returns>
        Task<int> InsertVehicleAsync(Vehicle vehicle);

        /// <summary>
        /// Updates an existing vehicle in the database.
        /// </summary>
        /// <param name="vehicle">The vehicle with updated data.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        Task<bool> UpdateVehicleAsync(Vehicle vehicle);

    }

}