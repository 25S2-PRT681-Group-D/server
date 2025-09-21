using AgroScan.API.DTOs;
using AgroScan.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroScan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InspectionAnalysisController : ControllerBase
    {
        private readonly IInspectionAnalysisService _analysisService;

        public InspectionAnalysisController(IInspectionAnalysisService analysisService)
        {
            _analysisService = analysisService;
        }

        [HttpGet("inspection/{inspectionId}")]
        public async Task<ActionResult<InspectionAnalysisDto>> GetAnalysisByInspection(int inspectionId)
        {
            var analysis = await _analysisService.GetAnalysisByInspectionIdAsync(inspectionId);
            if (analysis == null)
            {
                return NotFound();
            }

            return Ok(analysis);
        }

        [HttpPost]
        public async Task<ActionResult<InspectionAnalysisDto>> CreateAnalysis(CreateInspectionAnalysisDto createAnalysisDto)
        {
            try
            {
                var analysis = await _analysisService.CreateAnalysisAsync(createAnalysisDto);
                return CreatedAtAction(nameof(GetAnalysisByInspection), new { inspectionId = analysis.InspectionId }, analysis);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("inspection/{inspectionId}")]
        public async Task<ActionResult<InspectionAnalysisDto>> UpdateAnalysis(int inspectionId, UpdateInspectionAnalysisDto updateAnalysisDto)
        {
            var analysis = await _analysisService.UpdateAnalysisAsync(inspectionId, updateAnalysisDto);
            if (analysis == null)
            {
                return NotFound();
            }

            return Ok(analysis);
        }

        [HttpDelete("inspection/{inspectionId}")]
        public async Task<ActionResult> DeleteAnalysis(int inspectionId)
        {
            var result = await _analysisService.DeleteAnalysisAsync(inspectionId);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
