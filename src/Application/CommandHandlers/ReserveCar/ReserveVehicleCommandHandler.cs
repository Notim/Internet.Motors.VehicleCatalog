using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Services.CreateNewOrderService;
using Domain;
using MediatR;
using Notim.Outputs;

namespace Application.CommandHandlers.ReserveCar
{
    public class ReserveVehicleCommandHandler : IRequestHandler<ReserveVehicleCommand, Output>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ICreateNewOrderService _createNewOrderService;

        public ReserveVehicleCommandHandler(
            IVehicleRepository vehicleRepository,
            ICreateNewOrderService createNewOrderService
        )
        {
            _vehicleRepository = vehicleRepository;
            _createNewOrderService = createNewOrderService;
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

            await _createNewOrderService.CreateNewOrderAsync(
                new CreateNewOrder(
                    orderId: Guid.NewGuid(), 
                    vehicleId: vehicle.VehicleId,
                    customerDocument: request.CustomerDocument,
                    carName: vehicle.CarName,
                    price: vehicle.Price!.Value
                ), 
                cancellationToken
            );
            
            vehicle.ReserveVehicle();
            
            var updateResult = await _vehicleRepository.UpdateVehicleAsync(vehicle);
            if (!updateResult)
            {
                output.AddFault(new Fault(FaultType.InvalidOperation, $"Failed to reserve the vehicle with ID {request.VehicleId}."));
                return output;
            }

            output.AddMessage($"Vehicle with ID {request.VehicleId} successfully reserved.");
            return output;
        }
    }
}