using Data.Dao.Dtos;
using Domain;

namespace Data.Mappers;

public static class VehicleMappers
{

    public static Vehicle MapToDomainModel(this VehicleDto? vehicleDto)
    {
        if (vehicleDto is null)
        {
            return null!;
        }

        return new Vehicle(
            id: vehicleDto.Id,
            vehicleId: vehicleDto.VehicleId,
            carName: vehicleDto.CarName,
            brand: vehicleDto.Brand,
            model: vehicleDto.Model,
            year: vehicleDto.Year,
            color: vehicleDto.Color,
            fuelType: vehicleDto.FuelType,
            numberOfDoors: vehicleDto.NumberOfDoors,
            mileage: vehicleDto.Mileage,
            price: vehicleDto.Price,
            saleDate: vehicleDto.SaleDate,
            status: Enum.Parse<SaleStatus>(vehicleDto.Status),
            isReserved: vehicleDto.IsReserved
        );
    }
    
    public static IEnumerable<Vehicle> MapToDomainModel(this IEnumerable<VehicleDto> vehicleDtos)
    {
        var vehicleList = new List<Vehicle>();
        foreach (var vehicleDto in vehicleDtos)
        {
            vehicleList.Add(vehicleDto.MapToDomainModel());
        }

        return vehicleList;
    }
    
    public static VehicleDto MapToDto(this Vehicle vehicle)
    {
        return new VehicleDto
        {
            Id = vehicle.Id,
            VehicleId = vehicle.VehicleId,
            CarName = vehicle.CarName,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Color = vehicle.Color,
            FuelType = vehicle.FuelType,
            NumberOfDoors = vehicle.NumberOfDoors,
            Mileage = vehicle.Mileage,
            Price = vehicle.Price,
            SaleDate = vehicle.SaleDate,
            Status = vehicle.Status.ToString(),
            IsReserved = vehicle.IsReserved
        };
    }

}