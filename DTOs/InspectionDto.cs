using System.ComponentModel.DataAnnotations;

namespace AgroScan.API.DTOs
{
    public class InspectionDto
    {
        public int Id { get; set; }
        public string PlantName { get; set; } = string.Empty;
        public DateTime InspectionDate { get; set; }
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UserId { get; set; }
        public List<InspectionImageDto> Images { get; set; } = new List<InspectionImageDto>();
        public InspectionAnalysisDto? Analysis { get; set; }
    }

    public class CreateInspectionDto
    {
        [Required]
        [MaxLength(100)]
        public string PlantName { get; set; } = string.Empty;

        [Required]
        public DateTime InspectionDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    public class UpdateInspectionDto
    {
        [MaxLength(100)]
        public string? PlantName { get; set; }

        public DateTime? InspectionDate { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}
