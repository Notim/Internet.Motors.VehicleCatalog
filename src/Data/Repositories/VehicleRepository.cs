using System.Data;
using Dapper;
using Data.Configs;
using Domain;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Data.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly DatabaseConfig _config;

        public VehicleRepository(IOptions<DatabaseConfig> configOptions)
        {
            _config = configOptions?.Value ?? throw new ArgumentNullException(nameof(configOptions));
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            using (IDbConnection db = CreateConnection())
            {
                const string query = "SELECT * FROM VEHICLE";
                return await db.QueryAsync<Vehicle>(query);
            }
        }

        public async Task<int> InsertVehicleAsync(Vehicle vehicle)
        {
            using (IDbConnection db = CreateConnection())
            {
                const string query = @"
                    INSERT INTO VEHICLE 
                    (VehicleId, CarName, Brand, Model, Year, Color, FuelType, NumberOfDoors, Mileage, Price, ManufacturingDate, SaleDate, SaleStatus, IsReserved)
                    VALUES
                    (@VehicleId, @CarName, @Brand, @Model, @Year, @Color, @FuelType, @NumberOfDoors, @Mileage, @Price, @ManufacturingDate, @SaleDate, @SaleStatus, @IsReserved);
                    SELECT CAST(SCOPE_IDENTITY() as INT);
                ";
                
                return await db.ExecuteScalarAsync<int>(query, vehicle);
            }
        }

        public async Task<bool> UpdateVehicleAsync(Vehicle vehicle)
        {
            using (IDbConnection db = CreateConnection())
            {
                const string query = @"
                    UPDATE VEHICLE
                    SET
                        CarName = @CarName,
                        Brand = @Brand,
                        Model = @Model,
                        Year = @Year,
                        Color = @Color,
                        FuelType = @FuelType,
                        NumberOfDoors = @NumberOfDoors,
                        Mileage = @Mileage,
                        Price = @Price,
                        ManufacturingDate = @ManufacturingDate,
                        SaleDate = @SaleDate,
                        SaleStatus = @SaleStatus,
                        IsReserved = @IsReserved
                    WHERE Id = @Id;
                ";
                
                var rowsAffected = await db.ExecuteAsync(query, vehicle);
                
                return rowsAffected > 0;
            }
        }
        
        private IDbConnection CreateConnection()
        {
            var connection = new SqlConnection(_config.ConnectionString);
            connection.Open();
            
            return connection;
        }
    }
}