using System.ComponentModel.DataAnnotations;

namespace AgroScan.API.DTOs
{
    public class InspectionImageDto
    {
        public int Id { get; set; }
        public int InspectionId { get; set; }
        public string Image { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateInspectionImageDto
    {
        [Required]
        public int InspectionId { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; } = null!;
    }

    public class CreateMultipleInspectionImagesDto
    {
        [Required]
        public int InspectionId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one image is required")]
        public List<IFormFile> ImageFiles { get; set; } = new List<IFormFile>();
    }
}
