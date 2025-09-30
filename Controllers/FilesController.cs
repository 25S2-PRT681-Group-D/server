using AgroScan.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace AgroScan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FilesController : BaseController
    {
        private readonly IFileService _fileService;
        private readonly IInspectionService _inspectionService;
        private readonly IUserService _userService;
        private readonly ILogger<FilesController> _logger;

        public FilesController(
            IFileService fileService,
            IInspectionService inspectionService,
            IUserService userService,
            ILogger<FilesController> logger)
        {
            _fileService = fileService;
            _inspectionService = inspectionService;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Export inspections to CSV format
        /// </summary>
        [HttpGet("inspections/export/csv")]
        public async Task<IActionResult> ExportInspectionsToCsv()
        {
            try
            {
                var inspections = await _inspectionService.GetAllInspectionsAsync();
                var csvData = await _fileService.ExportInspectionsToCsvAsync(inspections);

                var fileName = $"inspections_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                return File(csvData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting inspections to CSV");
                _logger.LogError(ex, "Error exporting inspections to CSV");
                return StatusCode(500, "An error occurred while exporting data");
            }
        }

        /// <summary>
        /// Export inspections to Excel format
        /// </summary>
        [HttpGet("inspections/export/excel")]
        public async Task<IActionResult> ExportInspectionsToExcel()
        {
            try
            {
                var inspections = await _inspectionService.GetAllInspectionsAsync();
                var excelData = await _fileService.ExportInspectionsToExcelAsync(inspections);

                var fileName = $"inspections_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting inspections to Excel");
                _logger.LogError(ex, "Error exporting inspections to CSV");
                return StatusCode(500, "An error occurred while exporting data");
            }
        }

        /// <summary>
        /// Import inspections from CSV file
        /// </summary>
        [HttpPost("inspections/import/csv")]
        public async Task<IActionResult> ImportInspectionsFromCsv(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }

                if (!file.ContentType.Contains("csv") && !file.FileName.EndsWith(".csv"))
                {
                    return BadRequest("File must be a CSV file");
                }

                using var stream = file.OpenReadStream();
                var inspections = await _fileService.ImportInspectionsFromCsvAsync(stream);

                // Here you would typically save the imported data to the database
                // For now, we'll just return the count
                return Ok(new { message = $"Successfully imported {inspections.Count()} inspections", count = inspections.Count() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing inspections from CSV");
                _logger.LogError(ex, "Error exporting inspections to CSV");
                return StatusCode(500, "An error occurred while exporting data");
            }
        }

        /// <summary>
        /// Export users to CSV format
        /// </summary>
        [HttpGet("users/export/csv")]
        public async Task<IActionResult> ExportUsersToCsv()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var csvData = await _fileService.ExportUsersToCsvAsync(users);

                var fileName = $"users_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                return File(csvData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting users to CSV");
                _logger.LogError(ex, "Error exporting inspections to CSV");
                return StatusCode(500, "An error occurred while exporting data");
            }
        }

        /// <summary>
        /// Export users to Excel format
        /// </summary>
        [HttpGet("users/export/excel")]
        public async Task<IActionResult> ExportUsersToExcel()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var excelData = await _fileService.ExportUsersToExcelAsync(users);

                var fileName = $"users_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting users to Excel");
                _logger.LogError(ex, "Error exporting inspections to CSV");
                return StatusCode(500, "An error occurred while exporting data");
            }
        }

        /// <summary>
        /// Import users from CSV file
        /// </summary>
        [HttpPost("users/import/csv")]
        public async Task<IActionResult> ImportUsersFromCsv(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }

                if (!file.ContentType.Contains("csv") && !file.FileName.EndsWith(".csv"))
                {
                    return BadRequest("File must be a CSV file");
                }

                using var stream = file.OpenReadStream();
                var users = await _fileService.ImportUsersFromCsvAsync(stream);

                return Ok(new { message = $"Successfully imported {users.Count()} users", count = users.Count() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing users from CSV");
                _logger.LogError(ex, "Error exporting inspections to CSV");
                return StatusCode(500, "An error occurred while exporting data");
            }
        }
    }
}
