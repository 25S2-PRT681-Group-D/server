using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroScan.API.Models
{
    public class Inspection
    {
        [Key]
        public int Id { get; set; }

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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual ICollection<InspectionImage> InspectionImages { get; set; } = new List<InspectionImage>();
        public virtual InspectionAnalysis? InspectionAnalysis { get; set; }
    }
}
