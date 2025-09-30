namespace AgroScan.API.Services
{
    public interface IBackgroundTaskService
    {
        Task EnqueueTaskAsync<T>(T taskData, string taskName);
        Task EnqueueTaskAsync<T>(T taskData, string taskName, TimeSpan delay);
        Task EnqueueTaskAsync<T>(T taskData, string taskName, DateTime scheduledTime);
        Task<bool> IsTaskQueuedAsync(string taskId);
        Task CancelTaskAsync(string taskId);
        Task<int> GetQueueCountAsync();
    }
}
