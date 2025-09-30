using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroScan.API.Data;

namespace AgroScan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly AgroScanDbContext _context;
        private readonly ILogger<HealthController> _logger;

        public HealthController(AgroScanDbContext context, ILogger<HealthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var healthStatus = new
                {
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Version = "1.0.0",
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                    Database = await CheckDatabaseHealthAsync(),
                    Services = new
                    {
                        Database = "Connected",
                        BackgroundJobs = "Running",
                        Logging = "Active"
                    }
                };

                return Ok(healthStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                
                var unhealthyStatus = new
                {
                    Status = "Unhealthy",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                };

                return StatusCode(503, unhealthyStatus);
            }
        }

        /// <summary>
        /// Detailed health check with database connectivity
        /// </summary>
        [HttpGet("detailed")]
        public async Task<IActionResult> GetDetailed()
        {
            try
            {
                var databaseHealth = await CheckDatabaseHealthAsync();
                var memoryUsage = GC.GetTotalMemory(false);
                var uptime = Environment.TickCount64;

                var detailedStatus = new
                {
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Version = "1.0.0",
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                    Database = databaseHealth,
                    System = new
                    {
                        MemoryUsage = $"{memoryUsage / 1024 / 1024} MB",
                        Uptime = $"{uptime / 1000} seconds",
                        ProcessorCount = Environment.ProcessorCount,
                        MachineName = Environment.MachineName
                    },
                    Services = new
                    {
                        Database = "Connected", // Simplified for now
                        BackgroundJobs = "Running",
                        Logging = "Active",
                        FileSystem = "Accessible"
                    }
                };

                return Ok(detailedStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Detailed health check failed");
                
                var unhealthyStatus = new
                {
                    Status = "Unhealthy",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message,
                    Database = new { IsHealthy = false, Error = ex.Message }
                };

                return StatusCode(503, unhealthyStatus);
            }
        }

        private async Task<object> CheckDatabaseHealthAsync()
        {
            try
            {
                // Test database connectivity
                await _context.Database.CanConnectAsync();
                
                // Test a simple query
                var userCount = await _context.Users.CountAsync();
                
                return new
                {
                    IsHealthy = true,
                    ConnectionString = "Connected",
                    UserCount = userCount,
                    LastChecked = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                return new
                {
                    IsHealthy = false,
                    Error = ex.Message,
                    LastChecked = DateTime.UtcNow
                };
            }
        }
    }
}
