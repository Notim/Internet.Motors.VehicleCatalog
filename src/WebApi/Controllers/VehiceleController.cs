using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Internet.Motors.VehicleCatalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehiclesController(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        /// <summary>
        /// Recebe os dados do veículo e realiza o cadastro no banco de dados.
        /// </summary>
        /// <param name="vehicle">Informações do veículo a serem cadastradas.</param>
        /// <returns>O ID do veículo recém-cadastrado.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterVehicle([FromBody] Vehicle? vehicle)
        {
            if (vehicle == null)
                return BadRequest("Vehicle data is required.");

            try
            {
                if (string.IsNullOrWhiteSpace(vehicle.CarName) || string.IsNullOrWhiteSpace(vehicle.Brand))
                {
                    return BadRequest("Vehicle Name and Brand are required.");
                }

                vehicle.VehicleId = vehicle.VehicleId == Guid.Empty ? Guid.NewGuid() : vehicle.VehicleId;

                var newVehicleId = await _vehicleRepository.InsertVehicleAsync(vehicle);

                return CreatedAtAction(
                    nameof(RegisterVehicle),
                    new
                    {
                        id = newVehicleId
                    },
                    newVehicleId
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}