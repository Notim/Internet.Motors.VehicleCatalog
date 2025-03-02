using Domain;

namespace Application.CommandHandlers.RegisterVehicle
{

    public static class RegisterVehicleMappers
    {

        public static Vehicle MapToDomainVehicle(this RegisterVehicleCommand request)
        {
            var vehicle = new Vehicle(
                id: 0,
                vehicleId: Guid.NewGuid(),
                carName: request.CarName,
                brand: request.Brand,
                model: request.Model,
                year: request.Year,
                color: request.Color,
                fuelType: request.FuelType,
                numberOfDoors: request.NumberOfDoors,
                mileage: request.Mileage,
                price: request.Price,
                status: SaleStatus.Available
            );

            return vehicle;
        }

    }

}