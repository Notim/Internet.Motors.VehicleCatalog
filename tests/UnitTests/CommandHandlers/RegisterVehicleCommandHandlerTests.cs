using Application.CommandHandlers.RegisterVehicle;
using Domain;
using FluentAssertions;
using Moq;

namespace UnitTests.CommandHandlers;

public class RegisterVehicleCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRegisterVehicle_WhenCommandIsValid()
    {
        // Arrange
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        mockVehicleRepository.Setup(repo => repo.InsertVehicleAsync(It.IsAny<Vehicle>()))
                             .ReturnsAsync(1);

        var handler = new RegisterVehicleCommandHandler(mockVehicleRepository.Object);

        var command = new RegisterVehicleCommand
        {
            CarName = "Test Car",
            Brand = "Test Brand",
            Model = "Test Model",
            Year = 2022,
            Color = "Red",
            FuelType = "Gasoline",
            NumberOfDoors = 4,
            Mileage = 15000,
            Price = 25000
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Messages.Should().Contain("vehicle added with id: 1");
        
        mockVehicleRepository.Verify(repo => repo.InsertVehicleAsync(It.IsAny<Vehicle>()), Times.Once);
    }
    
}