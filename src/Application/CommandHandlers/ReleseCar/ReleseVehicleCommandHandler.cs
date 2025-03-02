using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Notim.Outputs;

namespace Application.CommandHandlers.ReleseCar
{
    public class ReleaseVehicleCommandHandler : IRequestHandler<ReleaseVehicleCommand, Output>
    {
        private readonly IVehicleRepository _vehicleRepository;

        public ReleaseVehicleCommandHandler(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public async Task<Output> Handle(ReleaseVehicleCommand request, CancellationToken cancellationToken)
        {
            var output = new Output();

            var vehicle = await _vehicleRepository.GetVehicleByVehicleId(request.VehicleId);

            if (vehicle == null)
            {
                output.AddFault(new Fault(FaultType.ResourceNotFound, $"Vehicle with ID {request.VehicleId} not found."));
                return output;
            }

            if (vehicle.Status == SaleStatus.Available)
            {
                output.AddFault(new Fault(FaultType.InvalidOperation, $"Vehicle with ID {request.VehicleId} is already available."));
                return output;
            }

            if (vehicle.Status != SaleStatus.Reserved)
            {
                output.AddFault(new Fault(FaultType.InvalidOperation, $"Vehicle with ID {request.VehicleId} is not available for release."));
                return output;
            }

            vehicle.ReleaseVehicle();

            var updateResult = await _vehicleRepository.UpdateVehicleAsync(vehicle);

            if (!updateResult)
            {
                output.AddFault(new Fault(FaultType.InvalidOperation, $"Failed to release the vehicle with ID {request.VehicleId}."));
                return output;
            }

            // TODO: publish message on kafka topic car-reserved

            output.AddMessage($"Vehicle with ID {request.VehicleId} successfully released.");
            return output;
        }
    }
}