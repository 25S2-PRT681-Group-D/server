using AgroScan.API.Data;
using AgroScan.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgroScan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedDataController : ControllerBase
    {
        private readonly AgroScanDbContext _context;

        public SeedDataController(AgroScanDbContext context)
        {
            _context = context;
        }

        [HttpPost("analytics/{userId}")]
        public async Task<ActionResult> SeedAnalyticsData(int userId)
        {
            try
            {
                await SeedDataService.SeedAnalyticsDataAsync(_context, userId);
                return Ok(new { message = $"Successfully seeded 200 inspection records for user {userId}" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while seeding data", error = ex.Message });
            }
        }

        [HttpPost("reset")]
        public async Task<ActionResult> ResetData()
        {
            try
            {
                // Clear existing data
                _context.InspectionAnalyses.RemoveRange(_context.InspectionAnalyses);
                _context.Inspections.RemoveRange(_context.Inspections);
                _context.Users.RemoveRange(_context.Users);
                await _context.SaveChangesAsync();

                // Seed fresh data
                await SeedDataService.SeedDataAsync(_context);
                await SeedDataService.SeedAnalyticsDataAsync(_context, 1); // Seed for first user

                return Ok(new { message = "Database reset and fresh data seeded successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while resetting data", error = ex.Message });
            }
        }
    }
}
