using AgroScan.API.DTOs;
using AgroScan.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgroScan.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class InspectionsController : BaseController
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
            try
            {
                var userId = GetCurrentUserId();
                var inspections = await _inspectionService.SearchInspectionsAsync(userId, plantName, status, startDate, endDate);
                return Ok(inspections);
            }
            catch (Exception ex)
            {
                return HandleServiceException(ex);
            }
        }

        [HttpGet("my-inspections")]
        public async Task<ActionResult<IEnumerable<InspectionDto>>> GetMyInspections()
        {
            try
            {
                var userId = GetCurrentUserId();
                var inspections = await _inspectionService.GetInspectionsByUserIdAsync(userId);
                return Ok(inspections);
            }
            catch (Exception ex)
            {
                return HandleServiceException(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InspectionDto>> GetInspection(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Invalid inspection ID" });
                }

                var inspection = await _inspectionService.GetInspectionByIdAsync(id);
                return HandleServiceResult(inspection, "Inspection not found");
            }
            catch (Exception ex)
            {
                return HandleServiceException(ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<InspectionDto>> CreateInspection(CreateInspectionDto createInspectionDto)
        {
            try
            {
                var validationResult = ValidateModelState();
                if (validationResult != null) return validationResult;

                var userId = GetCurrentUserId();
                var inspection = await _inspectionService.CreateInspectionAsync(createInspectionDto, userId);
                return CreatedAtAction(nameof(GetInspection), new { id = inspection.Id }, inspection);
            }
            catch (Exception ex)
            {
                return HandleServiceException(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InspectionDto>> UpdateInspection(int id, UpdateInspectionDto updateInspectionDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Invalid inspection ID" });
                }

                var validationResult = ValidateModelState();
                if (validationResult != null) return validationResult;

                var inspection = await _inspectionService.UpdateInspectionAsync(id, updateInspectionDto);
                return HandleServiceResult(inspection, "Inspection not found");
            }
            catch (Exception ex)
            {
                return HandleServiceException(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInspection(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Invalid inspection ID" });
                }

                var result = await _inspectionService.DeleteInspectionAsync(id);
                return HandleServiceResult(result, "Inspection not found");
            }
            catch (Exception ex)
            {
                return HandleServiceException(ex);
            }
        }
    }
}
