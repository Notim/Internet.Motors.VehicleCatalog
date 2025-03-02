using System;
using MediatR;
using Notim.Outputs;

namespace Application.CommandHandlers.ReleseCar
{

    public record ReleaseVehicleCommand : IRequest<Output>
    {

        public Guid VehicleId { get; set; }

    }

}