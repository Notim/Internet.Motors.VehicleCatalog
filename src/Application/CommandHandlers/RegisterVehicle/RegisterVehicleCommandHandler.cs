using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Notim.Outputs;

namespace Application.CommandHandlers.RegisterVehicle
{
    public class RegisterVehicleCommandHandler : IRequestHandler<RegisterVehicleCommand, Output>
    {
        private readonly IVehicleRepository _vehicleRepository;

        public RegisterVehicleCommandHandler(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
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
                Status = SaleStatus.Available
            };

            var vehicleId = await _vehicleRepository.InsertVehicleAsync(vehicle);

            output.AddMessage($"vehicle added with id: {vehicleId}");

            return output;
        }
    }
}