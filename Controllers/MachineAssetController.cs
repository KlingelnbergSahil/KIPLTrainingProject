using KlingelnbergMachineManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KlingelnbergMachineManagement.Controllers
{
    [ApiController]
    [Route("api")]
    public class MachineAssetController : ControllerBase
    {
        private readonly IMachineService _service;
        private readonly ILogger<MachineAssetController> _logger;

        
        public MachineAssetController(
            IMachineService service,
            ILogger<MachineAssetController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("machines")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllMachineTypes()
        {
            try
            {
                return Ok(await _service.GetAllMachineTypesAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all machines");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("machines/latest-series")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMachinesWithLatestSeries()
        {
            try
            {
                return Ok(await _service.GetMachinesWithLatestServicesAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting machines with latest series");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("machines/details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllMachineDetails()
        {
            try
            {
                return Ok(await _service.GetAllMachineDetailsAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting machine details");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("machines/by-asset")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMachinesByAsset(
            [FromQuery] string assetName)
        {
            if (string.IsNullOrWhiteSpace(assetName))
                return BadRequest("assetName is required");

            try
            {
                var machines = await _service.GetMachinesByAssetNameAsync(assetName);

                if (!machines.Any())
                    return NotFound($"No machines found using asset '{assetName}'");

                return Ok(machines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting machines by asset {AssetName}", assetName);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("assets/by-machine")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAssetsByMachine(
            [FromQuery] string machineType)
        {
            if (string.IsNullOrWhiteSpace(machineType))
                return BadRequest("machineType is required");

            try
            {
                var assets = await _service.GetAssetsByMachineTypeAsync(machineType);

                if (!assets.Any())
                    return NotFound($"No assets found for machine '{machineType}'");

                return Ok(assets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting assets for machine {MachineType}", machineType);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("assets")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAssets()
        {
            try
            {
                return Ok(await _service.GetAllAssetNamesAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all assets");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
