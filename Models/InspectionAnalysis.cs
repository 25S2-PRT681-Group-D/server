using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroScan.API.Models
{
    public class InspectionAnalysis
    {
        [Key]
        [ForeignKey("Inspection")]
        public int InspectionId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty; // 'Healthy', 'At Risk', 'Diseased', etc.

        [Required]
        [Range(0, 100)]
        public decimal ConfidenceScore { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string TreatmentRecommendation { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Inspection Inspection { get; set; } = null!;
    }
}
