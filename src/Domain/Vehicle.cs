using System;

namespace Domain
{

    public class Vehicle
    {

        public int Id { get; set; }

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

        public SaleStatus? Status { get; set; }

        public bool IsReserved { get; set; }
        
        public void ReserveVehicle()
        {
            Status = SaleStatus.Reserved;
            IsReserved = true;
        }
        
        public void SellVehicle()
        {
            Status = SaleStatus.Sold;
            IsReserved = false;
            SaleDate = DateTime.Now;
        }
        
        public void ReleaseVehicle()
        {
            Status = SaleStatus.Available;
            IsReserved = false;
            SaleDate = null;
        }

    }

}