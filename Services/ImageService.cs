using AgroScan.API.Data;
using AgroScan.API.DTOs;
using AgroScan.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AgroScan.API.Services
{
    public class ImageService : IImageService
    {
        private readonly AgroScanDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly string _imagePath;

        public ImageService(AgroScanDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
            _imagePath = Path.Combine(_environment.WebRootPath, "images", "inspections");
            
            // Ensure the directory exists
            if (!Directory.Exists(_imagePath))
            {
                Directory.CreateDirectory(_imagePath);
            }
        }

        public async Task<InspectionImageDto> UploadImageAsync(CreateInspectionImageDto createImageDto)
        {
            // Validate file
            if (createImageDto.ImageFile == null || createImageDto.ImageFile.Length == 0)
            {
                throw new ArgumentException("No file uploaded.");
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var fileExtension = Path.GetExtension(createImageDto.ImageFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("Invalid file type. Only JPG, PNG, and WEBP files are allowed.");
            }

            // Validate file size (10MB max)
            if (createImageDto.ImageFile.Length > 10 * 1024 * 1024)
            {
                throw new ArgumentException("File size too large. Maximum size is 10MB.");
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_imagePath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await createImageDto.ImageFile.CopyToAsync(stream);
            }

            // Save to database
            var inspectionImage = new InspectionImage
            {
                InspectionId = createImageDto.InspectionId,
                Image = fileName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.InspectionImages.Add(inspectionImage);
            await _context.SaveChangesAsync();

            return new InspectionImageDto
            {
                Id = inspectionImage.Id,
                InspectionId = inspectionImage.InspectionId,
                Image = inspectionImage.Image,
                CreatedAt = inspectionImage.CreatedAt,
                UpdatedAt = inspectionImage.UpdatedAt
            };
        }

        public async Task<bool> DeleteImageAsync(int imageId)
        {
            var image = await _context.InspectionImages.FindAsync(imageId);
            if (image == null) return false;

            // Delete physical file
            var filePath = Path.Combine(_imagePath, image.Image);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Delete from database
            _context.InspectionImages.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<InspectionImageDto?> GetImageByIdAsync(int imageId)
        {
            var image = await _context.InspectionImages.FindAsync(imageId);
            return image != null ? new InspectionImageDto
            {
                Id = image.Id,
                InspectionId = image.InspectionId,
                Image = image.Image,
                CreatedAt = image.CreatedAt,
                UpdatedAt = image.UpdatedAt
            } : null;
        }

        public async Task<IEnumerable<InspectionImageDto>> GetImagesByInspectionIdAsync(int inspectionId)
        {
            var images = await _context.InspectionImages
                .Where(img => img.InspectionId == inspectionId)
                .ToListAsync();

            return images.Select(img => new InspectionImageDto
            {
                Id = img.Id,
                InspectionId = img.InspectionId,
                Image = img.Image,
                CreatedAt = img.CreatedAt,
                UpdatedAt = img.UpdatedAt
            });
        }

        public Task<string> GetImagePathAsync(string imageName)
        {
            var filePath = Path.Combine(_imagePath, imageName);
            return Task.FromResult(File.Exists(filePath) ? filePath : string.Empty);
        }
    }
}
