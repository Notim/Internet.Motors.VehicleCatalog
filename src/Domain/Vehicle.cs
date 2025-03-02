namespace Domain
{

    public class Vehicle
    {

        public Vehicle(
            long id,
            Guid vehicleId,
            string? carName,
            string? brand,
            string? model,
            int year, 
            string? color,
            string? fuelType,
            int numberOfDoors,
            decimal mileage,
            decimal price, 
            DateTime? saleDate = null,
            SaleStatus? status = null,
            bool? isReserved = null
        )
        {
            Id = id;
            VehicleId = vehicleId;
            CarName = carName;
            Brand = brand;
            Model = model;
            Year = year;
            Color = color;
            FuelType = fuelType;
            NumberOfDoors = numberOfDoors;
            Mileage = mileage;
            Price = price;
            SaleDate = saleDate;
            Status = status ?? SaleStatus.Available;
            IsReserved = isReserved ?? false;
        }

        public long Id { get; private set; }

        public Guid VehicleId { get; private set; }

        public string? CarName { get; private set; }

        public string? Brand { get; private set; }

        public string? Model { get; private set; }

        public int Year { get; private set; }

        public string? Color { get; private set; }

        public string? FuelType { get; private set; }

        public int NumberOfDoors { get; private set; }

        public decimal Mileage { get; private set; }

        public decimal Price { get; private set; }

        public DateTime? SaleDate { get; private set; }

        public SaleStatus? Status { get; private set; }

        public bool IsReserved { get; private set; }
        
        public void SetId(long id)
        {
            Id = id;
        }
        
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