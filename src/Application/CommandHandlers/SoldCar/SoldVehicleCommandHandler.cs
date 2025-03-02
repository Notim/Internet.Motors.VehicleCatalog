using System;
using System.Threading;
using System.Threading.Tasks;
using Application.CommandHandlers.ReserveCar;
using Domain;
using MediatR;
using Notim.Outputs;

namespace Application.CommandHandlers.SoldCar
{
    public class SoldVehicleCommandHandler : IRequestHandler<SoldVehicleCommand, Output>
    {
        private readonly IVehicleRepository _vehicleRepository;

        public SoldVehicleCommandHandler(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public async Task<Output> Handle(SoldVehicleCommand request, CancellationToken cancellationToken)
        {
            var output = new Output();

            var vehicle = await _vehicleRepository.GetVehicleByVehicleId(request.VehicleId);

            if (vehicle == null)
            {
                output.AddFault(new Fault(FaultType.ResourceNotFound, $"Vehicle with ID {request.VehicleId} not found."));
                return output;
            }

            if (vehicle.Status == SaleStatus.Sold)
            {
                output.AddFault(new Fault(FaultType.InvalidOperation, $"Vehicle with ID {request.VehicleId} is already marked as sold."));
                return output;
            }

            if (vehicle.Status != SaleStatus.Reserved)
            {
                output.AddFault(new Fault(FaultType.InvalidOperation, $"Vehicle with ID {request.VehicleId} is not available for marked as sold."));
                return output;
            }

            vehicle.SellVehicle();

            var updateResult = await _vehicleRepository.UpdateVehicleAsync(vehicle);

            if (!updateResult)
            {
                output.AddFault(new Fault(FaultType.InvalidOperation, $"Failed to mark the vehicle with ID {request.VehicleId} as sold."));
                return output;
            }

            output.AddMessage($"Vehicle with ID {request.VehicleId} has been marked as sold.");
            return output;
        }
    }
}