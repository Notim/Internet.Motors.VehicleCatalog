using System;
using MediatR;
using Notim.Outputs;

namespace Application.CommandHandlers.SoldCar
{

    public record SoldVehicleCommand : IRequest<Output>
    {

        public Guid VehicleId { get; set; }

    }

}