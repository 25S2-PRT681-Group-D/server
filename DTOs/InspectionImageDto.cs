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
}
