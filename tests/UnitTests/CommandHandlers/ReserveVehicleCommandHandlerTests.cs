using Application.CommandHandlers.ReserveCar;
using Domain;
using FluentAssertions;
using Moq;

namespace UnitTests.CommandHandlers;

public class ReserveVehicleCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReserveVehicle_WhenVehicleIsAvailable()
    {
        // Arrange
        var mockVehicleRepository = new Mock<IVehicleRepository>();
        
        var vehicleId = Guid.NewGuid();
        var testVehicle = new Vehicle
        {
            VehicleId = vehicleId,
            Status = SaleStatus.Available,
            IsReserved = false
        };

        mockVehicleRepository.Setup(repo => repo.GetVehicleByVehicleId(vehicleId))
                             .ReturnsAsync(testVehicle);

        mockVehicleRepository.Setup(repo => repo.UpdateVehicleAsync(It.IsAny<Vehicle>()))
                             .ReturnsAsync(true);

        var handler = new ReserveVehicleCommandHandler(mockVehicleRepository.Object);
        var command = new ReserveVehicleCommand
        {
            VehicleId = vehicleId,
            CustomerDocument = "test"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Messages.Should().Contain($"Vehicle with ID {vehicleId} successfully reserved.");

        testVehicle.Status.Should().Be(SaleStatus.Reserved);
        testVehicle.IsReserved.Should().BeTrue();

        mockVehicleRepository.Verify(repo => repo.GetVehicleByVehicleId(vehicleId), Times.Once);
        mockVehicleRepository.Verify(repo => repo.UpdateVehicleAsync(It.IsAny<Vehicle>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenVehicleIsAlreadyReserved()
    {
        // Arrange
        var mockVehicleRepository = new Mock<IVehicleRepository>();
        
        var vehicleId = Guid.NewGuid();
        var testVehicle = new Vehicle
        {
            VehicleId = vehicleId,
            Status = SaleStatus.Reserved,
            IsReserved = true
        };

        mockVehicleRepository.Setup(repo => repo.GetVehicleByVehicleId(vehicleId))
                             .ReturnsAsync(testVehicle);

        var handler = new ReserveVehicleCommandHandler(mockVehicleRepository.Object);
        var command = new ReserveVehicleCommand
        {
            VehicleId = vehicleId,
            CustomerDocument = "test"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.FaultMessages.Should().Contain($"Vehicle with ID {vehicleId} is already reserved.");

        mockVehicleRepository.Verify(repo => repo.GetVehicleByVehicleId(vehicleId), Times.Once);
        mockVehicleRepository.Verify(repo => repo.UpdateVehicleAsync(It.IsAny<Vehicle>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenVehicleIsNotAvailable()
    {
        // Arrange
        var mockVehicleRepository = new Mock<IVehicleRepository>();
        
        var vehicleId = Guid.NewGuid();
        var testVehicle = new Vehicle
        {
            VehicleId = vehicleId,
            Status = SaleStatus.Sold, // Not available
            IsReserved = false
        };

        mockVehicleRepository.Setup(repo => repo.GetVehicleByVehicleId(vehicleId))
                             .ReturnsAsync(testVehicle);

        var handler = new ReserveVehicleCommandHandler(mockVehicleRepository.Object);
        var command = new ReserveVehicleCommand
        {
            VehicleId = vehicleId,
            CustomerDocument = "test"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.FaultMessages.Should().Contain($"Vehicle with ID {vehicleId} is not available for reservation.");

        mockVehicleRepository.Verify(repo => repo.GetVehicleByVehicleId(vehicleId), Times.Once);
        mockVehicleRepository.Verify(repo => repo.UpdateVehicleAsync(It.IsAny<Vehicle>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenVehicleDoesNotExist()
    {
        // Arrange
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        var vehicleId = Guid.NewGuid();

        mockVehicleRepository.Setup(repo => repo.GetVehicleByVehicleId(vehicleId))
                             .ReturnsAsync((Vehicle)null); // Simulate not found

        var handler = new ReserveVehicleCommandHandler(mockVehicleRepository.Object);
        var command = new ReserveVehicleCommand
        {
            VehicleId = vehicleId,
            CustomerDocument = "test"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.FaultMessages.Should().Contain($"Vehicle with ID {vehicleId} not found.");

        mockVehicleRepository.Verify(repo => repo.GetVehicleByVehicleId(vehicleId), Times.Once);
        mockVehicleRepository.Verify(repo => repo.UpdateVehicleAsync(It.IsAny<Vehicle>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUpdateFails()
    {
        // Arrange
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        var vehicleId = Guid.NewGuid();
        var testVehicle = new Vehicle
        {
            VehicleId = vehicleId,
            Status = SaleStatus.Available,
            IsReserved = false
        };

        mockVehicleRepository.Setup(repo => repo.GetVehicleByVehicleId(vehicleId))
                             .ReturnsAsync(testVehicle);

        mockVehicleRepository.Setup(repo => repo.UpdateVehicleAsync(It.IsAny<Vehicle>()))
                             .ReturnsAsync(false);

        var handler = new ReserveVehicleCommandHandler(mockVehicleRepository.Object);
        var command = new ReserveVehicleCommand
        {
            VehicleId = vehicleId,
            CustomerDocument = "test"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.FaultMessages.Should().Contain($"Failed to reserve the vehicle with ID {vehicleId}.");

        mockVehicleRepository.Verify(repo => repo.GetVehicleByVehicleId(vehicleId), Times.Once);
        mockVehicleRepository.Verify(repo => repo.UpdateVehicleAsync(It.IsAny<Vehicle>()), Times.Once);
    }
}