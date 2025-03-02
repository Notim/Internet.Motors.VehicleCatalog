using Application.CommandHandlers.RegisterVehicle;
using Application.CommandHandlers.ReleseCar;
using Application.CommandHandlers.ReserveCar;
using Application.CommandHandlers.SoldCar;
using Application.QueryHandlers.ListAllVehicles;
using Domain;
using FluentAssertions;
using Internet.Motors.VehicleCatalog.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using Notim.Outputs;

namespace UnitTests.WebApi
{
    public class VehiclesControllerTests
    {
        private readonly AutoMocker _mocker;
        private readonly VehiclesController _controller;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<VehiclesController>> _loggerMock;

        public VehiclesControllerTests()
        {
            _mocker = new AutoMocker();
            _mediatorMock = _mocker.GetMock<IMediator>();
            _loggerMock = _mocker.GetMock<ILogger<VehiclesController>>();

            _controller = _mocker.CreateInstance<VehiclesController>();
        }

        [Fact]
        public async Task RegisterVehicleAction_Should_Return_BadRequest_When_Command_Is_Null()
        {
            // Act
            var result = await _controller.RegisterVehicleAction(null, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Vehicle data is required.");
        }

        [Fact]
        public async Task RegisterVehicleAction_Should_Return_BadRequest_When_Command_Fields_Are_Invalid()
        {
            // Arrange
            var command = new RegisterVehicleCommand { CarName = "", Brand = "" };

            // Act
            var result = await _controller.RegisterVehicleAction(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Vehicle Name and Brand are required.");
        }

        [Fact]
        public async Task RegisterVehicleAction_Should_Return_Created_When_Command_Executes_Successfully()
        {
            // Arrange
            var command = new RegisterVehicleCommand { CarName = "TestCar", Brand = "TestBrand" };
            var output = new Output();
            
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(output);

            // Act
            var result = await _controller.RegisterVehicleAction(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<CreatedResult>();
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RegisterVehicleAction_Should_Return_BadRequest_When_CommandResult_IsInvalid()
        {
            // Arrange
            var output = new Output();
            output.AddFault(new Fault(FaultType.GenericError,"Error occurred.") );
            
            var command = new RegisterVehicleCommand { CarName = "TestCar", Brand = "TestBrand" };

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(output);

            // Act
            var result = await _controller.RegisterVehicleAction(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RegisterVehicleAction_Should_Log_Critical_On_Exception()
        {
            // Arrange
            var command = new RegisterVehicleCommand { CarName = "TestCar", Brand = "TestBrand" };
            var exception = new Exception("Database connection issue");
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ThrowsAsync(exception);

            // Act
            var result = await _controller.RegisterVehicleAction(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error while registering vehicle.")),
                    exception,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllVehiclesAction_Should_Return_Ok_When_Service_Returns_Valid_Data()
        {
            // Arrange
            var output = new Output<IEnumerable<VehicleViewModel>>();
            output.AddResult(new List<VehicleViewModel>());
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<ListAllVehiclesQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(output);

            // Act
            var result = await _controller.GetAllVehiclesAction(SaleStatus.Available, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Fact]
        public async Task GetAllVehiclesAction_Should_Return_BadRequest_When_Service_Returns_InValid_Data()
        {
            // Arrange
            var output = new Output<IEnumerable<VehicleViewModel>>();
            output.AddFault(new Fault(FaultType.GenericError, "Error occurred."));
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<ListAllVehiclesQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(output);

            // Act
            var result = await _controller.GetAllVehiclesAction(SaleStatus.Available, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ReserveVehicleAction_Should_Return_Accepted_When_Command_Executes_Successfully()
        {
            // Arrange
            var output = new Output();
            
            var command = new ReserveVehicleCommand { VehicleId = Guid.NewGuid() };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(output);

            // Act
            var result = await _controller.ReserveVehicleAction(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<AcceptedResult>();
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SoldVehicleAction_Should_Handle_Exception_And_Log_Critical()
        {
            // Arrange
            var command = new SoldVehicleCommand { VehicleId = Guid.NewGuid() };
            var exception = new Exception("Error processing command");

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ThrowsAsync(exception);

            // Act
            var result = await _controller.SoldVehicleAction(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(500);
            
            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error while mark as sold vehicle.")),
                    exception,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task ReleaseVehicleAction_Should_Return_BadRequest_If_CommandResult_IsInvalid()
        {
            // Arrange
            var output = new Output();
            output.AddFault(new Fault(FaultType.InvalidOperation, "Failed to release vehicle" ));
            
            var command = new ReleaseVehicleCommand { VehicleId = Guid.NewGuid() };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(output);

            // Act
            var result = await _controller.ReleaseVehicleAction(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().BeEquivalentTo(new List<string> { "Failed to release vehicle" });
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}