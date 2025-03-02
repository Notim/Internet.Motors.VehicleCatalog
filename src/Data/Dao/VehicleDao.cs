using System.Data;
using Dapper;
using Data.Configs;
using Data.Dao.Dtos;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Data.Dao;

public class VehicleDao : IVehicleDao
{

    private readonly DatabaseConfig _config;

    public VehicleDao(IOptions<DatabaseConfig> configOptions)
    {
        _config = configOptions?.Value ?? throw new ArgumentNullException(nameof(configOptions));
    }

    public async Task<VehicleDto?> GetVehicleByVehicleId(Guid vehicleId)
    {
        using (IDbConnection db = CreateConnection())
        {
            const string query = @"
                SELECT * 
                FROM VEHICLE 
                WHERE VehicleId = @VehicleId;
            ";

            return await db.QueryFirstOrDefaultAsync<VehicleDto?>(
                query,
                new
                {
                    VehicleId = vehicleId
                }
            );
        }
    }

    public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
    {
        using (IDbConnection db = CreateConnection())
        {
            const string query = "SELECT * FROM VEHICLE";
            return await db.QueryAsync<VehicleDto>(query);
        }
    }

    public async Task<int> InsertVehicleAsync(VehicleDto vehicle)
    {
        using (IDbConnection db = CreateConnection())
        {
            const string query = @"
                INSERT INTO VEHICLE 
                (VehicleId, CarName, Brand, Model, Year, Color, FuelType, NumberOfDoors, Mileage, Price, SaleDate, Status, IsReserved)
                VALUES
                (@VehicleId, @CarName, @Brand, @Model, @Year, @Color, @FuelType, @NumberOfDoors, @Mileage, @Price, @SaleDate, @Status, @IsReserved);
                SELECT CAST(SCOPE_IDENTITY() as INT);
            ";

            return await db.ExecuteScalarAsync<int>(query, vehicle);
        }
    }

    public async Task<bool> UpdateVehicleAsync(VehicleDto vehicle)
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
                    SaleDate = @SaleDate,
                    Status = @Status,
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