using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Notim.Outputs;

namespace Application.QueryHandlers.ListAllVehicles
{
    public class ListAllVehiclesQueryHandler : IRequestHandler<ListAllVehiclesQuery, Output<IEnumerable<VehicleViewModel>>>
    {
        private readonly IVehicleRepository _vehicleRepository;

        public ListAllVehiclesQueryHandler(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        }

        public async Task<Output<IEnumerable<VehicleViewModel>>> Handle(ListAllVehiclesQuery request, CancellationToken cancellationToken)
        {
            var output = new Output<IEnumerable<VehicleViewModel>>();
            
            var vehicles = (await _vehicleRepository.GetAllVehiclesAsync()).ToList();

            if (!vehicles.Any())
                return output;
            
            vehicles = vehicles.OrderByDescending(x => x.Price).ToList();
            
            if (request.Status is not null)
                vehicles = vehicles.Where(x => x.Status == request.Status).ToList();

            var vehicleViewModels = new List<VehicleViewModel>();
            
            foreach (var vehicle in vehicles)
            {
                vehicleViewModels.Add(new VehicleViewModel
                {
                    VehicleId = vehicle.VehicleId,
                    CarName = vehicle.CarName,
                    Brand = vehicle.Brand,
                    Model = vehicle.Model,
                    Year = vehicle.Year,
                    Color = vehicle.Color,
                    FuelType = vehicle.FuelType,
                    NumberOfDoors = vehicle.NumberOfDoors,
                    Mileage = vehicle.Mileage,
                    Price = vehicle.Price,
                    SaleStatus = vehicle.Status.ToString(),
                    IsReserved = vehicle.IsReserved,
                    SaleDate = vehicle.SaleDate
                });
            }
            
            output.AddMessages("vehicles found with success");
            output.AddResult(vehicleViewModels);

            return output;
        }
    }
}