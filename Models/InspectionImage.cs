using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroScan.API.Models
{
    public class InspectionImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Inspection")]
        public int InspectionId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Image { get; set; } = string.Empty; // Image filename

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Inspection Inspection { get; set; } = null!;
    }
}
