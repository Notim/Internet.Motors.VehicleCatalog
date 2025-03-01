using Domain;
using MediatR;
using Notim.Outputs;

namespace Application.CommandHandlers.ReserveCar
{
    public class ReserveVehicleCommandHandler : IRequestHandler<ReserveVehicleCommand, Output>
    {
        private readonly IVehicleRepository _vehicleRepository;

        public ReserveVehicleCommandHandler(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        }

        public async Task<Output> Handle(ReserveVehicleCommand request, CancellationToken cancellationToken)
        {
            var output = new Output();

            var vehicle = await _vehicleRepository.GetVehicleByVehicleId(request.VehicleId);

            if (vehicle == null)
            {
                output.AddFault(new Fault(FaultType.ResourceNotFound, $"Vehicle with ID {request.VehicleId} not found."));
                return output;
            }

            if (vehicle.Status == SaleStatus.Reserved)
            {
                output.AddFault(new Fault(FaultType.InvalidOperation, $"Vehicle with ID {request.VehicleId} is already reserved."));
                return output;
            }

            if (vehicle.Status != SaleStatus.Available)
            {
                output.AddFault(new Fault(FaultType.InvalidOperation, $"Vehicle with ID {request.VehicleId} is not available for reservation."));
                return output;
            }

            vehicle.ReserveVehicle();

            var updateResult = await _vehicleRepository.UpdateVehicleAsync(vehicle);

            if (!updateResult)
            {
                output.AddFault(new Fault(FaultType.InvalidOperation, $"Failed to reserve the vehicle with ID {request.VehicleId}."));
                return output;
            }

            // TODO: publish message on kafka topic car-reserved

            output.AddMessage($"Vehicle with ID {request.VehicleId} successfully reserved.");
            return output;
        }
    }
}