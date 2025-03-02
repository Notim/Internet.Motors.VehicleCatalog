using Bogus;
using Domain;

namespace UnitTests.Fakers
{
    public static class VehicleFaker
    {
        public static Faker<Vehicle> GetFaker()
        {
            return new Faker<Vehicle>().CustomInstantiator(f => new Vehicle(
                                        id: f.Random.Long(1, 1000),
                                        vehicleId: Guid.NewGuid(),
                                        carName: f.Vehicle.Model(),
                                        brand: f.Vehicle.Manufacturer(),
                                        model: f.Commerce.ProductName(),
                                        year: f.Date.Past(20).Year,
                                        color: f.Commerce.Color(),
                                        fuelType: f.PickRandom(
                                            "Petrol", 
                                            "Diesel",
                                            "Etanol",
                                            "Flex",
                                            "Electric",
                                            "Hybrid"
                                        ),
                                        numberOfDoors: f.Random.Int(2, 5),
                                        mileage: f.Random.Decimal(5000, 200000),
                                        price: f.Random.Decimal(10000, 100000),
                                        saleDate: f.Date.Past(),
                                        status: f.PickRandom<SaleStatus>(),
                                        isReserved: f.Random.Bool()
                                    ));
        }
    }
}