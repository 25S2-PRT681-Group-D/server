using AgroScan.API.DTOs;
using AgroScan.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgroScan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InspectionsController : ControllerBase
    {
        private readonly IInspectionService _inspectionService;

        public InspectionsController(IInspectionService inspectionService)
        {
            _inspectionService = inspectionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InspectionDto>>> GetInspections(
            [FromQuery] string? plantName,
            [FromQuery] string? status,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var inspections = await _inspectionService.SearchInspectionsAsync(userId, plantName, status, startDate, endDate);
            return Ok(inspections);
        }

        [HttpGet("my-inspections")]
        public async Task<ActionResult<IEnumerable<InspectionDto>>> GetMyInspections()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var inspections = await _inspectionService.GetInspectionsByUserIdAsync(userId);
            return Ok(inspections);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InspectionDto>> GetInspection(int id)
        {
            var inspection = await _inspectionService.GetInspectionByIdAsync(id);
            if (inspection == null)
            {
                return NotFound();
            }

            return Ok(inspection);
        }

        [HttpPost]
        public async Task<ActionResult<InspectionDto>> CreateInspection(CreateInspectionDto createInspectionDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var inspection = await _inspectionService.CreateInspectionAsync(createInspectionDto, userId);
            return CreatedAtAction(nameof(GetInspection), new { id = inspection.Id }, inspection);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InspectionDto>> UpdateInspection(int id, UpdateInspectionDto updateInspectionDto)
        {
            var inspection = await _inspectionService.UpdateInspectionAsync(id, updateInspectionDto);
            if (inspection == null)
            {
                return NotFound();
            }

            return Ok(inspection);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInspection(int id)
        {
            var result = await _inspectionService.DeleteInspectionAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
