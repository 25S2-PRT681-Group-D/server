using System.ComponentModel.DataAnnotations;

namespace AgroScan.API.DTOs
{
    public class InspectionAnalysisDto
    {
        public int InspectionId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal ConfidenceScore { get; set; }
        public string Description { get; set; } = string.Empty;
        public string TreatmentRecommendation { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateInspectionAnalysisDto
    {
        [Required]
        public int InspectionId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        [Required]
        [Range(0, 100)]
        public decimal ConfidenceScore { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string TreatmentRecommendation { get; set; } = string.Empty;
    }

    public class UpdateInspectionAnalysisDto
    {
        [MaxLength(50)]
        public string? Status { get; set; }

        [Range(0, 100)]
        public decimal? ConfidenceScore { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(2000)]
        public string? TreatmentRecommendation { get; set; }
    }
}
