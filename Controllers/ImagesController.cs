using AgroScan.API.DTOs;
using AgroScan.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroScan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ImagesController : ControllerBase
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
                var image = await _imageService.UploadImageAsync(createImageDto);
                return Ok(image);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InspectionImageDto>> GetImage(int id)
        {
            var image = await _imageService.GetImageByIdAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            return Ok(image);
        }

        [HttpGet("inspection/{inspectionId}")]
        public async Task<ActionResult<IEnumerable<InspectionImageDto>>> GetImagesByInspection(int inspectionId)
        {
            var images = await _imageService.GetImagesByInspectionIdAsync(inspectionId);
            return Ok(images);
        }

        [HttpGet("file/{imageName}")]
        public async Task<IActionResult> GetImageFile(string imageName)
        {
            var imagePath = await _imageService.GetImagePathAsync(imageName);
            if (string.IsNullOrEmpty(imagePath))
            {
                return NotFound();
            }

            var imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
            var contentType = GetContentType(imageName);
            return File(imageBytes, contentType);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteImage(int id)
        {
            var result = await _imageService.DeleteImageAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
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
