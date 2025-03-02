using Application.CommandHandlers.RegisterVehicle;
using Application.CommandHandlers.ReleseCar;
using Application.CommandHandlers.ReserveCar;
using Application.CommandHandlers.SoldCar;
using Application.QueryHandlers.ListAllVehicles;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApi.Controllers
{
    [ApiController]
    [Route("api/vehicle")]
    public class VehiclesController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController(
            IMediator mediator,
            ILogger<VehiclesController> logger
        )
        {
            _mediator = mediator;
            _logger = logger;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> RegisterVehicleAction([FromBody] RegisterVehicleCommand? vehicle, CancellationToken cancellationToken)
        {
            if (vehicle == null)
                return BadRequest("Vehicle data is required.");

            try
            {
                if (string.IsNullOrWhiteSpace(vehicle.CarName) || string.IsNullOrWhiteSpace(vehicle.Brand))
                {
                    return BadRequest("Vehicle Name and Brand are required.");
                }

                var output = await _mediator.Send(vehicle, cancellationToken);
                
                _logger.LogInformation("Output Returns {@Output}", output);
                
                if (!output.IsValid)
                {
                    return BadRequest(output.FaultMessages);
                }

                return Created();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while registering vehicle.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllVehiclesAction([FromQuery] SaleStatus? saleStatus, CancellationToken cancellationToken)
        {
            try
            {
                var output = await _mediator.Send(new ListAllVehiclesQuery
                {
                    Status = saleStatus
                }, cancellationToken);
                
                _logger.LogInformation("Output Returns {@Output}", output);
                
                if (!output.IsValid)
                {
                    return BadRequest(output.FaultMessages);
                }

                return Ok(output.Result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while registering vehicle.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        [HttpPost("{vehicleId:Guid}/reserve")]
        public async Task<IActionResult> ReserveVehicleAction([FromBody] ReserveVehicleCommand reserveVehicleRequest, CancellationToken cancellationToken)
        {
            try
            {
                var output = await _mediator.Send(reserveVehicleRequest, cancellationToken);
                
                _logger.LogInformation("Output Returns {@Output}", output);
                
                if (!output.IsValid)
                {
                    return BadRequest(output.FaultMessages);
                }

                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while registering vehicle.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        [HttpPost("{vehicleId:Guid}/sell")] // TODO: use kafka worker
        public async Task<IActionResult> SoldVehicleAction([FromBody] SoldVehicleCommand soldVehicleRequest, CancellationToken cancellationToken)
        {
            try
            {
                var output = await _mediator.Send(soldVehicleRequest, cancellationToken);
                
                _logger.LogInformation("Output Returns {@Output}", output);
                
                if (!output.IsValid)
                {
                    return BadRequest(output.FaultMessages);
                }

                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while mark as sold vehicle.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        [HttpPost("{vehicleId:Guid}/release")] // TODO: use kafka worker
        public async Task<IActionResult> ReleaseVehicleAction([FromBody] ReleaseVehicleCommand releaseVehicleCommand, CancellationToken cancellationToken)
        {
            try
            {
                var output = await _mediator.Send(releaseVehicleCommand, cancellationToken);
                
                _logger.LogInformation("Output Returns {@Output}", output);
                
                if (!output.IsValid)
                {
                    return BadRequest(output.FaultMessages);
                }

                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while registering vehicle.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}