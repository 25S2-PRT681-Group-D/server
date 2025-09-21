using AgroScan.API.DTOs;

namespace AgroScan.API.Services
{
    public interface IInspectionService
    {
        Task<InspectionDto?> GetInspectionByIdAsync(int id);
        Task<IEnumerable<InspectionDto>> GetInspectionsByUserIdAsync(int userId);
        Task<IEnumerable<InspectionDto>> GetAllInspectionsAsync();
        Task<InspectionDto> CreateInspectionAsync(CreateInspectionDto createInspectionDto, int userId);
        Task<InspectionDto?> UpdateInspectionAsync(int id, UpdateInspectionDto updateInspectionDto);
        Task<bool> DeleteInspectionAsync(int id);
        Task<IEnumerable<InspectionDto>> SearchInspectionsAsync(int userId, string? plantName, string? status, DateTime? startDate, DateTime? endDate);
    }
}
