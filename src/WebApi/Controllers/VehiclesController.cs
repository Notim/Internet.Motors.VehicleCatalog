using Application.CommandHandlers.RegisterVehicle;
using Application.CommandHandlers.ReserveCar;
using Application.QueryHandlers.ListAllVehicles;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Internet.Motors.VehicleCatalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> RegisterVehicle([FromBody] RegisterVehicleCommand? vehicle, CancellationToken cancellationToken)
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
        public async Task<IActionResult> GetAllVehicles([FromQuery] SaleStatus? saleStatus, CancellationToken cancellationToken)
        {
            try
            {
                var output = await _mediator.Send(new ListAllVehiclesQuery
                {
                    Status = saleStatus
                }, cancellationToken);
                
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
        
        [HttpPost]
        public async Task<IActionResult> ReserveVehicleVehicles([FromBody] ReserveVehicleCommand reserveVehicleRequest, CancellationToken cancellationToken)
        {
            try
            {
                var output = await _mediator.Send(reserveVehicleRequest, cancellationToken);
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