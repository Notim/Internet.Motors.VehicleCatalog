using System.Collections.Generic;
using Domain;
using MediatR;
using Notim.Outputs;

namespace Application.QueryHandlers.ListAllVehicles
{

    public record ListAllVehiclesQuery : IRequest<Output<IEnumerable<VehicleViewModel>>>
    {

        public SaleStatus? Status { get; set; }

    }

}