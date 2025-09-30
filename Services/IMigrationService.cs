namespace AgroScan.API.Services
{
    public interface IMigrationService
    {
        Task RunMigrationsAsync();
        Task RollbackMigrationAsync(long version);
        Task<long> GetCurrentVersionAsync();
        Task<IEnumerable<long>> GetMigrationHistoryAsync();
    }
}
