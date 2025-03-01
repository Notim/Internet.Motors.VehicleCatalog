using Domain;
using MediatR;
using Notim.Outputs;

namespace Application.CommandHandlers
{
    public class RegisterVehicleCommandHandler : IRequestHandler<RegisterVehicleCommand, Output>
    {
        private readonly IVehicleRepository _vehicleRepository;

        public RegisterVehicleCommandHandler(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        }

        public async Task<Output> Handle(RegisterVehicleCommand request, CancellationToken cancellationToken)
        {
            var output = new Output();
            
            var vehicle = new Vehicle
            {
                VehicleId = Guid.NewGuid(),
                CarName = request.CarName,
                Brand = request.Brand,
                Model = request.Model,
                Year = request.Year,
                Color = request.Color,
                FuelType = request.FuelType,
                NumberOfDoors = request.NumberOfDoors,
                Mileage = request.Mileage,
                Price = request.Price,
                ManufacturingDate = request.ManufacturingDate,
                SaleDate = request.SaleDate,
                SaleStatus = request.SaleStatus,
                IsReserved = request.IsReserved
            };

            var vehicleId = await _vehicleRepository.InsertVehicleAsync(vehicle);

            output.AddMessage($"vehicle added with id: {vehicleId}");

            return output;
        }
    }
}