using AgroScan.API.DTOs;

namespace AgroScan.API.Services
{
    public interface IImageService
    {
        Task<InspectionImageDto> UploadImageAsync(CreateInspectionImageDto createImageDto);
        Task<IEnumerable<InspectionImageDto>> UploadMultipleImagesAsync(CreateMultipleInspectionImagesDto createImagesDto);
        Task<bool> DeleteImageAsync(int imageId);
        Task<InspectionImageDto?> GetImageByIdAsync(int imageId);
        Task<IEnumerable<InspectionImageDto>> GetImagesByInspectionIdAsync(int inspectionId);
        Task<string> GetImagePathAsync(string imageName);
    }
}
