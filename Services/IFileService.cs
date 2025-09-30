using AgroScan.API.DTOs;

namespace AgroScan.API.Services
{
    public interface IFileService
    {
        Task<byte[]> ExportInspectionsToCsvAsync(IEnumerable<InspectionDto> inspections);
        Task<byte[]> ExportInspectionsToExcelAsync(IEnumerable<InspectionDto> inspections);
        Task<IEnumerable<InspectionDto>> ImportInspectionsFromCsvAsync(Stream csvStream);
        Task<byte[]> ExportUsersToCsvAsync(IEnumerable<UserDto> users);
        Task<byte[]> ExportUsersToExcelAsync(IEnumerable<UserDto> users);
        Task<IEnumerable<UserDto>> ImportUsersFromCsvAsync(Stream csvStream);
    }
}
