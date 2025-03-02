using Application.QueryHandlers.ListAllVehicles;
using Domain;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using UnitTests.Fakers;

namespace UnitTests.QueryHandlers;

public class ListAllVehiclesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnVehicles_WhenVehiclesExist()
    {
        // Arrange
        var mocker = new AutoMocker();

        var testVehicles = VehicleFaker.GetFaker().Generate(2);

        // Mock repository behavior to return a list of vehicles.
        mocker.GetMock<IVehicleRepository>()
              .Setup(repo => repo.GetAllVehiclesAsync())
              .ReturnsAsync(testVehicles);

        var handler = mocker.CreateInstance<ListAllVehiclesQueryHandler>();
        var query = new ListAllVehiclesQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Result.Should().HaveCount(2);
        foreach (var vehicle in testVehicles)
        {
            result.Result.Should().ContainSingle(v =>
                v.VehicleId == vehicle.VehicleId &&
                v.Status == vehicle.Status.ToString() &&
                v.IsReserved == vehicle.IsReserved &&
                v.CarName == vehicle.CarName &&
                v.Brand == vehicle.Brand &&
                v.Model == vehicle.Model &&
                v.Year == vehicle.Year &&
                v.Color == vehicle.Color &&
                v.FuelType == vehicle.FuelType &&
                v.NumberOfDoors == vehicle.NumberOfDoors &&
                v.Mileage == vehicle.Mileage &&
                v.Price == vehicle.Price &&
                v.SaleDate == vehicle.SaleDate
            );
        } 

        // Verify repository interactions.
        mocker.GetMock<IVehicleRepository>().Verify(repo => repo.GetAllVehiclesAsync(), Times.Once);
        
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoVehiclesExist()
    {
        // Arrange
        var mocker = new AutoMocker();

        // Mock repository to return an empty list.
        mocker.GetMock<IVehicleRepository>()
              .Setup(repo => repo.GetAllVehiclesAsync())
              .ReturnsAsync(new List<Vehicle>());

        var handler = mocker.CreateInstance<ListAllVehiclesQueryHandler>();
        var query = new ListAllVehiclesQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Result.Should().BeNull();

        // Verify repository interactions.
        mocker.GetMock<IVehicleRepository>().Verify(repo => repo.GetAllVehiclesAsync(), Times.Once);
    }
    
    [Theory]
    [InlineData(SaleStatus.Reserved, 5)]
    [InlineData(SaleStatus.Available, 10)]
    [InlineData(SaleStatus.Sold, 5)]
    public async Task Handle_ShouldReturnOnlyFilteredVehicles_When_PassSaleStatus(SaleStatus saleStatus, int itemscount)
    {
        // Arrange
        var mocker = new AutoMocker();
        var vehicles = VehicleFaker.GetFaker().Generate(20);

        vehicles.Take(10).ToList().ForEach(v => v.ReleaseVehicle());
        vehicles.Skip(10).Take(5).ToList().ForEach(v => v.SellVehicle());
        vehicles.Skip(15).Take(5).ToList().ForEach(v => v.ReserveVehicle());

        // Mock repository to return the generated list.
        mocker.GetMock<IVehicleRepository>()
              .Setup(repo => repo.GetAllVehiclesAsync())
              .ReturnsAsync(vehicles);

        var handler = mocker.CreateInstance<ListAllVehiclesQueryHandler>();
        var query = new ListAllVehiclesQuery
        {
            Status = saleStatus
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Result.Should().NotBeEmpty();
        result.Result.Count().Should().Be(itemscount);

        // Verify repository interactions.
        mocker.GetMock<IVehicleRepository>().Verify(repo => repo.GetAllVehiclesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogAndThrowException_WhenRepositoryThrowsException()
    {
        // Arrange
        var mocker = new AutoMocker();

        // Mock repository to throw an exception.
        mocker.GetMock<IVehicleRepository>()
              .Setup(repo => repo.GetAllVehiclesAsync())
              .ThrowsAsync(new Exception("Database failure"));

        var handler = mocker.CreateInstance<ListAllVehiclesQueryHandler>();
        var query = new ListAllVehiclesQuery();

        // Act
        Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database failure");

        // Verify repository interactions.
        mocker.GetMock<IVehicleRepository>().Verify(repo => repo.GetAllVehiclesAsync(), Times.Once);
    }
}