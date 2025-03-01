using System;

namespace Application.QueryHandlers.ListAllVehicles
{

    public record VehicleViewModel
    {

        public Guid VehicleId { get; set; }

        public string CarName { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public int Year { get; set; }

        public string? Color { get; set; }

        public string FuelType { get; set; }

        public int NumberOfDoors { get; set; }

        public decimal Mileage { get; set; }

        public decimal? Price { get; set; }

        public DateTime? ManufacturingDate { get; set; }

        public DateTime? SaleDate { get; set; }

        public string? SaleStatus { get; set; }

        public bool IsReserved { get; set; }

    }

}