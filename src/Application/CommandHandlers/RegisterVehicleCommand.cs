using MediatR;
using Notim.Outputs;

namespace Application.CommandHandlers
{

    // Command que encapsula os dados de criação de um veículo
    public class RegisterVehicleCommand : IRequest<Output> // Retorna o ID do veículo criado
    {

        public string CarName { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Color { get; set; } = string.Empty;
        public string FuelType { get; set; } = string.Empty;
        public int NumberOfDoors { get; set; }
        public decimal Mileage { get; set; }
        public decimal Price { get; set; }
        public DateTime ManufacturingDate { get; set; }
        public DateTime? SaleDate { get; set; }
        public string SaleStatus { get; set; } = string.Empty;
        public bool IsReserved { get; set; }

    }

}