using MediatR;
using Notim.Outputs;

namespace Application.CommandHandlers.RegisterVehicle
{

    public record RegisterVehicleCommand : IRequest<Output>
    {

        public string CarName { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public int Year { get; set; }

        public string Color { get; set; }

        public string FuelType { get; set; }

        public int NumberOfDoors { get; set; }

        public decimal Mileage { get; set; }

        public decimal Price { get; set; }

    }

}