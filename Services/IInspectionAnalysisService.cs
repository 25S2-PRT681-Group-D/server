using AgroScan.API.DTOs;

namespace AgroScan.API.Services
{
    public interface IInspectionAnalysisService
    {
        Task<InspectionAnalysisDto?> GetAnalysisByInspectionIdAsync(int inspectionId);
        Task<InspectionAnalysisDto> CreateAnalysisAsync(CreateInspectionAnalysisDto createAnalysisDto);
        Task<InspectionAnalysisDto?> UpdateAnalysisAsync(int inspectionId, UpdateInspectionAnalysisDto updateAnalysisDto);
        Task<bool> DeleteAnalysisAsync(int inspectionId);
    }
}
