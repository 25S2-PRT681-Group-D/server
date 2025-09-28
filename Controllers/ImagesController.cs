using AgroScan.API.DTOs;
using AgroScan.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroScan.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ImagesController : BaseController
    {
        private readonly IImageService _imageService;

        public ImagesController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<InspectionImageDto>> UploadImage(CreateInspectionImageDto createImageDto)
        {
            try
            {
                var validationResult = ValidateModelState();
                if (validationResult != null) return validationResult;

                var image = await _imageService.UploadImageAsync(createImageDto);
                return Ok(image);
            }
            catch (Exception ex)
            {
                return HandleServiceException(ex);
            }
        }

        [HttpPost("upload-multiple")]
        public async Task<ActionResult<IEnumerable<InspectionImageDto>>> UploadMultipleImages(CreateMultipleInspectionImagesDto createImagesDto)
        {
            try
            {
                var validationResult = ValidateModelState();
                if (validationResult != null) return validationResult;

                var images = await _imageService.UploadMultipleImagesAsync(createImagesDto);
                return Ok(images);
            }
            catch (Exception ex)
            {
                return HandleServiceException(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InspectionImageDto>> GetImage(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Invalid image ID" });
                }

                var image = await _imageService.GetImageByIdAsync(id);
                return HandleServiceResult(image, "Image not found");
            }
            catch (Exception ex)
            {
                return HandleServiceException(ex);
            }
        }

        [HttpGet("inspection/{inspectionId}")]
        public async Task<ActionResult<IEnumerable<InspectionImageDto>>> GetImagesByInspection(int inspectionId)
        {
            try
            {
                if (inspectionId <= 0)
                {
                    return BadRequest(new { message = "Invalid inspection ID" });
                }

                var images = await _imageService.GetImagesByInspectionIdAsync(inspectionId);
                return Ok(images);
            }
            catch (Exception ex)
            {
                return HandleServiceException(ex);
            }
        }

        [HttpGet("file/{imageName}")]
        public async Task<IActionResult> GetImageFile(string imageName)
        {
            try
            {
                if (string.IsNullOrEmpty(imageName))
                {
                    return BadRequest(new { message = "Image name is required" });
                }

                var imagePath = await _imageService.GetImagePathAsync(imageName);
                if (string.IsNullOrEmpty(imagePath))
                {
                    return NotFound(new { message = "Image file not found" });
                }

                var imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
                var contentType = GetContentType(imageName);
                return File(imageBytes, contentType);
            }
            catch (Exception ex)
            {
                return HandleServiceException(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteImage(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Invalid image ID" });
                }

                var result = await _imageService.DeleteImageAsync(id);
                return HandleServiceResult(result, "Image not found");
            }
            catch (Exception ex)
            {
                return HandleServiceException(ex);
            }
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}
