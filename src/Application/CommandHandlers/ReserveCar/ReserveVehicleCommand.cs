using System;
using MediatR;
using Notim.Outputs;

namespace Application.CommandHandlers.ReserveCar
{

    public class ReserveVehicleCommand : IRequest<Output>
    {

        public string? CustomerDocument { get; set; }

        public Guid VehicleId { get; set; }

    }

}