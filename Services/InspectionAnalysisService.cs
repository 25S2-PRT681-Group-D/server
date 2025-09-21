using AgroScan.API.Data;
using AgroScan.API.DTOs;
using AgroScan.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AgroScan.API.Services
{
    public class InspectionAnalysisService : IInspectionAnalysisService
    {
        private readonly AgroScanDbContext _context;

        public InspectionAnalysisService(AgroScanDbContext context)
        {
            _context = context;
        }

        public async Task<InspectionAnalysisDto?> GetAnalysisByInspectionIdAsync(int inspectionId)
        {
            var analysis = await _context.InspectionAnalyses
                .FirstOrDefaultAsync(a => a.InspectionId == inspectionId);

            return analysis != null ? MapToAnalysisDto(analysis) : null;
        }

        public async Task<InspectionAnalysisDto> CreateAnalysisAsync(CreateInspectionAnalysisDto createAnalysisDto)
        {
            // Check if analysis already exists for this inspection
            var existingAnalysis = await _context.InspectionAnalyses
                .FirstOrDefaultAsync(a => a.InspectionId == createAnalysisDto.InspectionId);

            if (existingAnalysis != null)
            {
                throw new InvalidOperationException("Analysis already exists for this inspection.");
            }

            var analysis = new InspectionAnalysis
            {
                InspectionId = createAnalysisDto.InspectionId,
                Status = createAnalysisDto.Status,
                ConfidenceScore = createAnalysisDto.ConfidenceScore,
                Description = createAnalysisDto.Description,
                TreatmentRecommendation = createAnalysisDto.TreatmentRecommendation,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.InspectionAnalyses.Add(analysis);
            await _context.SaveChangesAsync();

            return MapToAnalysisDto(analysis);
        }

        public async Task<InspectionAnalysisDto?> UpdateAnalysisAsync(int inspectionId, UpdateInspectionAnalysisDto updateAnalysisDto)
        {
            var analysis = await _context.InspectionAnalyses
                .FirstOrDefaultAsync(a => a.InspectionId == inspectionId);

            if (analysis == null) return null;

            if (!string.IsNullOrEmpty(updateAnalysisDto.Status))
                analysis.Status = updateAnalysisDto.Status;

            if (updateAnalysisDto.ConfidenceScore.HasValue)
                analysis.ConfidenceScore = updateAnalysisDto.ConfidenceScore.Value;

            if (!string.IsNullOrEmpty(updateAnalysisDto.Description))
                analysis.Description = updateAnalysisDto.Description;

            if (!string.IsNullOrEmpty(updateAnalysisDto.TreatmentRecommendation))
                analysis.TreatmentRecommendation = updateAnalysisDto.TreatmentRecommendation;

            analysis.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToAnalysisDto(analysis);
        }

        public async Task<bool> DeleteAnalysisAsync(int inspectionId)
        {
            var analysis = await _context.InspectionAnalyses
                .FirstOrDefaultAsync(a => a.InspectionId == inspectionId);

            if (analysis == null) return false;

            _context.InspectionAnalyses.Remove(analysis);
            await _context.SaveChangesAsync();
            return true;
        }

        private static InspectionAnalysisDto MapToAnalysisDto(InspectionAnalysis analysis)
        {
            return new InspectionAnalysisDto
            {
                InspectionId = analysis.InspectionId,
                Status = analysis.Status,
                ConfidenceScore = analysis.ConfidenceScore,
                Description = analysis.Description,
                TreatmentRecommendation = analysis.TreatmentRecommendation,
                CreatedAt = analysis.CreatedAt,
                UpdatedAt = analysis.UpdatedAt
            };
        }
    }
}
