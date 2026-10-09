    using KlingelnbergMachineManagement.Application.Services;
    using Microsoft.AspNetCore.Mvc;

    namespace KlingelnbergMachineManagement.Controllers
    {
        [ApiController]
        [Route("api/machine-data")]
        public class MachineDataImportController : ControllerBase
        {
            private readonly IMachineDataImportService _importService;
            private readonly ILogger<MachineDataImportController> _logger;

            public MachineDataImportController(
                IMachineDataImportService importService,
                ILogger<MachineDataImportController> logger)
            {
                _importService = importService;
                _logger = logger;
            }

            /// <summary>
            /// Upload machine data file (Replace / Append)
            /// </summary>
            [HttpPost("import")]
            public async Task<IActionResult> Import(
                IFormFile file,
                [FromQuery] ImportMode mode)
            {
                if (file == null || file.Length == 0)
                    return BadRequest("File is required");

                try
                {
                    await _importService.ImportAsync(
                        file.OpenReadStream(),
                        file.FileName,
                        mode
                    );

                    return Ok(new
                    {
                        Message = "Data imported successfully",
                        Mode = mode.ToString()
                    });
                }
                catch (InvalidDataException ex)
                {
                    // 🔴 File content / parsing error
                    _logger.LogWarning(ex, "Invalid data in uploaded file");
                    return BadRequest(ex.Message);
                }
                catch (FormatException ex)
                {
                    _logger.LogWarning(ex, "Invalid format in uploaded file");
                    return BadRequest(ex.Message);
                }
                catch (NotSupportedException ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error importing machine data");
                    return StatusCode(500, ex.Message);
                }
            }
        }
    }
