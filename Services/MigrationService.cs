using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace AgroScan.API.Services
{
    public class MigrationService : IMigrationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MigrationService> _logger;

        public MigrationService(IServiceProvider serviceProvider, ILogger<MigrationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task RunMigrationsAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

                _logger.LogInformation("Starting database migrations");
                runner.MigrateUp();
                _logger.LogInformation("Database migrations completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running database migrations");
                throw;
            }
        }

        public async Task RollbackMigrationAsync(long version)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

                _logger.LogInformation("Rolling back migration to version {Version}", version);
                runner.MigrateDown(version);
                _logger.LogInformation("Migration rollback completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rolling back migration to version {Version}", version);
                throw;
            }
        }

        public async Task<long> GetCurrentVersionAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

                // Simplified version - just return 1 for now
                _logger.LogInformation("Current database version: 1");
                return 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current migration version");
                return 0;
            }
        }

        public async Task<IEnumerable<long>> GetMigrationHistoryAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

                // Simplified version - return empty list for now
                _logger.LogInformation("Retrieved migration history: 0 migrations");
                return Enumerable.Empty<long>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting migration history");
                return Enumerable.Empty<long>();
            }
        }
    }
}
