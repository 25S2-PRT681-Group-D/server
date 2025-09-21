using AgroScan.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AgroScan.API.Data
{
    public class AgroScanDbContext : DbContext
    {
        public AgroScanDbContext(DbContextOptions<AgroScanDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Inspection> Inspections { get; set; }
        public DbSet<InspectionImage> InspectionImages { get; set; }
        public DbSet<InspectionAnalysis> InspectionAnalyses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            });

            // Configure Inspection entity
            modelBuilder.Entity<Inspection>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PlantName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
                entity.Property(e => e.State).IsRequired().HasMaxLength(100);
                entity.Property(e => e.City).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Inspections)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure InspectionImage entity
            modelBuilder.Entity<InspectionImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Image).IsRequired().HasMaxLength(255);

                entity.HasOne(e => e.Inspection)
                    .WithMany(i => i.InspectionImages)
                    .HasForeignKey(e => e.InspectionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure InspectionAnalysis entity
            modelBuilder.Entity<InspectionAnalysis>(entity =>
            {
                entity.HasKey(e => e.InspectionId);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ConfidenceScore).IsRequired().HasColumnType("decimal(5,2)");
                entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.TreatmentRecommendation).IsRequired().HasMaxLength(2000);

                entity.HasOne(e => e.Inspection)
                    .WithOne(i => i.InspectionAnalysis)
                    .HasForeignKey<InspectionAnalysis>(e => e.InspectionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
