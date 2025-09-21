using AgroScan.API.Data;
using AgroScan.API.DTOs;
using AgroScan.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AgroScan.API.Services
{
    public class InspectionService : IInspectionService
    {
        private readonly AgroScanDbContext _context;

        public InspectionService(AgroScanDbContext context)
        {
            _context = context;
        }

        public async Task<InspectionDto?> GetInspectionByIdAsync(int id)
        {
            var inspection = await _context.Inspections
                .Include(i => i.InspectionImages)
                .Include(i => i.InspectionAnalysis)
                .FirstOrDefaultAsync(i => i.Id == id);

            return inspection != null ? MapToInspectionDto(inspection) : null;
        }

        public async Task<IEnumerable<InspectionDto>> GetInspectionsByUserIdAsync(int userId)
        {
            var inspections = await _context.Inspections
                .Include(i => i.InspectionImages)
                .Include(i => i.InspectionAnalysis)
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.InspectionDate)
                .ToListAsync();

            return inspections.Select(MapToInspectionDto);
        }

        public async Task<IEnumerable<InspectionDto>> GetAllInspectionsAsync()
        {
            var inspections = await _context.Inspections
                .Include(i => i.InspectionImages)
                .Include(i => i.InspectionAnalysis)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return inspections.Select(MapToInspectionDto);
        }

        public async Task<InspectionDto> CreateInspectionAsync(CreateInspectionDto createInspectionDto, int userId)
        {
            var inspection = new Inspection
            {
                PlantName = createInspectionDto.PlantName,
                InspectionDate = createInspectionDto.InspectionDate,
                Country = createInspectionDto.Country,
                State = createInspectionDto.State,
                City = createInspectionDto.City,
                Notes = createInspectionDto.Notes,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Inspections.Add(inspection);
            await _context.SaveChangesAsync();

            return MapToInspectionDto(inspection);
        }

        public async Task<InspectionDto?> UpdateInspectionAsync(int id, UpdateInspectionDto updateInspectionDto)
        {
            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection == null) return null;

            if (!string.IsNullOrEmpty(updateInspectionDto.PlantName))
                inspection.PlantName = updateInspectionDto.PlantName;

            if (updateInspectionDto.InspectionDate.HasValue)
                inspection.InspectionDate = updateInspectionDto.InspectionDate.Value;

            if (!string.IsNullOrEmpty(updateInspectionDto.Country))
                inspection.Country = updateInspectionDto.Country;

            if (!string.IsNullOrEmpty(updateInspectionDto.State))
                inspection.State = updateInspectionDto.State;

            if (!string.IsNullOrEmpty(updateInspectionDto.City))
                inspection.City = updateInspectionDto.City;

            if (updateInspectionDto.Notes != null)
                inspection.Notes = updateInspectionDto.Notes;

            inspection.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToInspectionDto(inspection);
        }

        public async Task<bool> DeleteInspectionAsync(int id)
        {
            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection == null) return false;

            _context.Inspections.Remove(inspection);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<InspectionDto>> SearchInspectionsAsync(int userId, string? plantName, string? status, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Inspections
                .Include(i => i.InspectionImages)
                .Include(i => i.InspectionAnalysis)
                .Where(i => i.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(plantName))
            {
                query = query.Where(i => i.PlantName.Contains(plantName));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(i => i.InspectionAnalysis != null && i.InspectionAnalysis.Status == status);
            }

            if (startDate.HasValue)
            {
                query = query.Where(i => i.InspectionDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(i => i.InspectionDate <= endDate.Value);
            }

            var inspections = await query
                .OrderByDescending(i => i.InspectionDate)
                .ToListAsync();

            return inspections.Select(MapToInspectionDto);
        }

        private static InspectionDto MapToInspectionDto(Inspection inspection)
        {
            return new InspectionDto
            {
                Id = inspection.Id,
                PlantName = inspection.PlantName,
                InspectionDate = inspection.InspectionDate,
                Country = inspection.Country,
                State = inspection.State,
                City = inspection.City,
                Notes = inspection.Notes,
                CreatedAt = inspection.CreatedAt,
                UpdatedAt = inspection.UpdatedAt,
                UserId = inspection.UserId,
                Images = inspection.InspectionImages.Select(img => new InspectionImageDto
                {
                    Id = img.Id,
                    InspectionId = img.InspectionId,
                    Image = img.Image,
                    CreatedAt = img.CreatedAt,
                    UpdatedAt = img.UpdatedAt
                }).ToList(),
                Analysis = inspection.InspectionAnalysis != null ? new InspectionAnalysisDto
                {
                    InspectionId = inspection.InspectionAnalysis.InspectionId,
                    Status = inspection.InspectionAnalysis.Status,
                    ConfidenceScore = inspection.InspectionAnalysis.ConfidenceScore,
                    Description = inspection.InspectionAnalysis.Description,
                    TreatmentRecommendation = inspection.InspectionAnalysis.TreatmentRecommendation,
                    CreatedAt = inspection.InspectionAnalysis.CreatedAt,
                    UpdatedAt = inspection.InspectionAnalysis.UpdatedAt
                } : null
            };
        }
    }
}
