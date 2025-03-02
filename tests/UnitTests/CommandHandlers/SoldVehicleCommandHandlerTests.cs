using Application.CommandHandlers.SoldCar;
using Domain;
using FluentAssertions;
using Moq;
using Moq.AutoMock;

namespace UnitTests.CommandHandlers;

public class SoldVehicleCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldMarkVehicleAsSold_WhenVehicleIsAvailable()
    {
        // Arrange
        var mocker = new AutoMocker();

        var vehicleId = Guid.NewGuid();
        var testVehicle = new Vehicle
        {
            VehicleId = vehicleId,
            Status = SaleStatus.Reserved,
            IsReserved = false
        };

        // Mock repository behavior
        mocker.GetMock<IVehicleRepository>()
              .Setup(repo => repo.GetVehicleByVehicleId(vehicleId))
              .ReturnsAsync(testVehicle);

        mocker.GetMock<IVehicleRepository>()
              .Setup(repo => repo.UpdateVehicleAsync(It.IsAny<Vehicle>()))
              .ReturnsAsync(true);

        var handler = mocker.CreateInstance<SoldVehicleCommandHandler>();
        var command = new SoldVehicleCommand
        {
            VehicleId = vehicleId,
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Messages.Should().Contain($"Vehicle with ID {vehicleId} has been marked as sold.");

        testVehicle.Status.Should().Be(SaleStatus.Sold);
        testVehicle.IsReserved.Should().BeFalse();

        // Verify repository interactions
        mocker.GetMock<IVehicleRepository>().Verify(repo => repo.GetVehicleByVehicleId(vehicleId), Times.Once);
        mocker.GetMock<IVehicleRepository>().Verify(repo => repo.UpdateVehicleAsync(It.IsAny<Vehicle>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenVehicleIsAlreadySold()
    {
        // Arrange
        var mocker = new AutoMocker();

        var vehicleId = Guid.NewGuid();
        var testVehicle = new Vehicle
        {
            VehicleId = vehicleId,
            Status = SaleStatus.Sold,
            IsReserved = false
        };

        // Mock repository behavior
        mocker.GetMock<IVehicleRepository>()
              .Setup(repo => repo.GetVehicleByVehicleId(vehicleId))
              .ReturnsAsync(testVehicle);

        var handler = mocker.CreateInstance<SoldVehicleCommandHandler>();
        var command = new SoldVehicleCommand
        {
            VehicleId = vehicleId,
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.FaultMessages.Should().Contain($"Vehicle with ID {vehicleId} is already marked as sold.");

        // Verify repository interactions
        mocker.GetMock<IVehicleRepository>().Verify(repo => repo.GetVehicleByVehicleId(vehicleId), Times.Once);
        mocker.GetMock<IVehicleRepository>().Verify(repo => repo.UpdateVehicleAsync(It.IsAny<Vehicle>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenVehicleIsAvailable()
    {
        // Arrange
        var mocker = new AutoMocker();

        var vehicleId = Guid.NewGuid();
        var testVehicle = new Vehicle
        {
            VehicleId = vehicleId,
            Status = SaleStatus.Available,
            IsReserved = false
        };

        // Mock repository behavior
        mocker.GetMock<IVehicleRepository>()
              .Setup(repo => repo.GetVehicleByVehicleId(vehicleId))
              .ReturnsAsync(testVehicle);

        var handler = mocker.CreateInstance<SoldVehicleCommandHandler>();
        var command = new SoldVehicleCommand
        {
            VehicleId = vehicleId,
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.FaultMessages.Should().Contain($"Vehicle with ID {vehicleId} is not available for marked as sold.");

        // Verify repository interactions
        mocker.GetMock<IVehicleRepository>().Verify(repo => repo.GetVehicleByVehicleId(vehicleId), Times.Once);
        mocker.GetMock<IVehicleRepository>().Verify(repo => repo.UpdateVehicleAsync(It.IsAny<Vehicle>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenVehicleDoesNotExist()
    {
        // Arrange
        var mocker = new AutoMocker();

        var vehicleId = Guid.NewGuid();

        // Mock repository behavior
        mocker.GetMock<IVehicleRepository>()
              .Setup(repo => repo.GetVehicleByVehicleId(vehicleId))
              .ReturnsAsync((Vehicle)null);

        var handler = mocker.CreateInstance<SoldVehicleCommandHandler>();
        var command = new SoldVehicleCommand
        {
            VehicleId = vehicleId,
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.FaultMessages.Should().Contain($"Vehicle with ID {vehicleId} not found.");

        // Verify repository interactions
        mocker.GetMock<IVehicleRepository>().Verify(repo => repo.GetVehicleByVehicleId(vehicleId), Times.Once);
        mocker.GetMock<IVehicleRepository>().Verify(repo => repo.UpdateVehicleAsync(It.IsAny<Vehicle>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUpdateFails()
    {
        // Arrange
        var mocker = new AutoMocker();

        var vehicleId = Guid.NewGuid();
        var testVehicle = new Vehicle
        {
            VehicleId = vehicleId,
            Status = SaleStatus.Reserved,
            IsReserved = false
        };

        // Mock repository behavior
        mocker.GetMock<IVehicleRepository>()
              .Setup(repo => repo.GetVehicleByVehicleId(vehicleId))
              .ReturnsAsync(testVehicle);

        mocker.GetMock<IVehicleRepository>()
              .Setup(repo => repo.UpdateVehicleAsync(It.IsAny<Vehicle>()))
              .ReturnsAsync(false); // Simulate an update failure

        var handler = mocker.CreateInstance<SoldVehicleCommandHandler>();
        var command = new SoldVehicleCommand
        {
            VehicleId = vehicleId,
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.FaultMessages.Should().Contain($"Failed to mark the vehicle with ID {vehicleId} as sold.");

        // Verify repository interactions
        mocker.GetMock<IVehicleRepository>().Verify(repo => repo.GetVehicleByVehicleId(vehicleId), Times.Once);
        mocker.GetMock<IVehicleRepository>().Verify(repo => repo.UpdateVehicleAsync(It.IsAny<Vehicle>()), Times.Once);
    }
}