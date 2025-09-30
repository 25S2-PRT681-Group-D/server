using AgroScan.API.Models;
using AgroScan.API.Utilities;
using AgroScan.API.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroScan.API.Services
{
    public class BackgroundTaskService : IBackgroundTaskService
    {
        private readonly AgroScanDbContext _context;
        private readonly ILogger<BackgroundTaskService> _logger;
        private readonly IEmailUtility _emailUtility;
        private readonly IWebApiUtility _webApiUtility;

        public BackgroundTaskService(
            AgroScanDbContext context,
            ILogger<BackgroundTaskService> logger,
            IEmailUtility emailUtility,
            IWebApiUtility webApiUtility)
        {
            _context = context;
            _logger = logger;
            _emailUtility = emailUtility;
            _webApiUtility = webApiUtility;
        }

        public async Task EnqueueTaskAsync<T>(T taskData, string taskName)
        {
            await EnqueueTaskAsync(taskData, taskName, TimeSpan.Zero);
        }

        public async Task EnqueueTaskAsync<T>(T taskData, string taskName, TimeSpan delay)
        {
            var scheduledTime = DateTime.UtcNow.Add(delay);
            await EnqueueTaskAsync(taskData, taskName, scheduledTime);
        }

        public async Task EnqueueTaskAsync<T>(T taskData, string taskName, DateTime scheduledTime)
        {
            try
            {
                var task = new BackgroundTask
                {
                    Id = Guid.NewGuid().ToString(),
                    TaskName = taskName,
                    TaskData = System.Text.Json.JsonSerializer.Serialize(taskData),
                    Status = "Queued",
                    CreatedAt = DateTime.UtcNow,
                    ScheduledAt = scheduledTime,
                    Attempts = 0,
                    MaxAttempts = 3
                };

                _context.BackgroundTasks.Add(task);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Task {TaskName} queued with ID {TaskId}", taskName, task.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enqueueing task {TaskName}", taskName);
                throw;
            }
        }

        public async Task<bool> IsTaskQueuedAsync(string taskId)
        {
            try
            {
                return await _context.BackgroundTasks
                    .AnyAsync(t => t.Id == taskId && t.Status == "Queued");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking task status for {TaskId}", taskId);
                return false;
            }
        }

        public async Task CancelTaskAsync(string taskId)
        {
            try
            {
                var task = await _context.BackgroundTasks
                    .FirstOrDefaultAsync(t => t.Id == taskId);

                if (task != null && task.Status == "Queued")
                {
                    task.Status = "Cancelled";
                    task.CompletedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Task {TaskId} cancelled", taskId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling task {TaskId}", taskId);
                throw;
            }
        }

        public async Task<int> GetQueueCountAsync()
        {
            try
            {
                return await _context.BackgroundTasks
                    .CountAsync(t => t.Status == "Queued");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting queue count");
                return 0;
            }
        }
    }

    // Background task processor
    public class BackgroundTaskProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BackgroundTaskProcessor> _logger;
        private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(30);

        public BackgroundTaskProcessor(IServiceProvider serviceProvider, ILogger<BackgroundTaskProcessor> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background task processor started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AgroScanDbContext>();
                    var emailUtility = scope.ServiceProvider.GetRequiredService<IEmailUtility>();
                    var webApiUtility = scope.ServiceProvider.GetRequiredService<IWebApiUtility>();

                    await ProcessTasksAsync(context, emailUtility, webApiUtility);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing background tasks");
                }

                await Task.Delay(_processingInterval, stoppingToken);
            }

            _logger.LogInformation("Background task processor stopped");
        }

        private async Task ProcessTasksAsync(AgroScanDbContext context, IEmailUtility emailUtility, IWebApiUtility webApiUtility)
        {
            var tasks = await context.BackgroundTasks
                .Where(t => t.Status == "Queued" && t.ScheduledAt <= DateTime.UtcNow)
                .OrderBy(t => t.ScheduledAt)
                .Take(10)
                .ToListAsync();

            foreach (var task in tasks)
            {
                try
                {
                    task.Status = "Processing";
                    task.StartedAt = DateTime.UtcNow;
                    task.Attempts++;
                    await context.SaveChangesAsync();

                    await ProcessTaskAsync(task, emailUtility, webApiUtility);

                    task.Status = "Completed";
                    task.CompletedAt = DateTime.UtcNow;
                    await context.SaveChangesAsync();

                    _logger.LogInformation("Task {TaskId} completed successfully", task.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing task {TaskId}", task.Id);

                    task.Status = task.Attempts >= task.MaxAttempts ? "Failed" : "Queued";
                    task.ErrorMessage = ex.Message;
                    task.ScheduledAt = DateTime.UtcNow.AddMinutes(Math.Pow(2, task.Attempts)); // Exponential backoff
                    await context.SaveChangesAsync();
                }
            }
        }

        private async Task ProcessTaskAsync(BackgroundTask task, IEmailUtility emailUtility, IWebApiUtility webApiUtility)
        {
            switch (task.TaskName)
            {
                case "SendEmail":
                    await ProcessEmailTaskAsync(task, emailUtility);
                    break;
                case "WebApiCall":
                    await ProcessWebApiTaskAsync(task, webApiUtility);
                    break;
                case "DataExport":
                    await ProcessDataExportTaskAsync(task);
                    break;
                default:
                    _logger.LogWarning("Unknown task type: {TaskName}", task.TaskName);
                    break;
            }
        }

        private async Task ProcessEmailTaskAsync(BackgroundTask task, IEmailUtility emailUtility)
        {
            var emailData = System.Text.Json.JsonSerializer.Deserialize<EmailTaskData>(task.TaskData);
            if (emailData != null)
            {
                await emailUtility.SendEmailAsync(
                    emailData.To,
                    emailData.Subject,
                    emailData.Body,
                    emailData.IsHtml
                );
            }
        }

        private async Task ProcessWebApiTaskAsync(BackgroundTask task, IWebApiUtility webApiUtility)
        {
            var apiData = System.Text.Json.JsonSerializer.Deserialize<WebApiTaskData>(task.TaskData);
            if (apiData != null)
            {
                var response = await webApiUtility.GetStringAsync(apiData.Url, apiData.Headers);
                _logger.LogInformation("Web API call completed: {Url}", apiData.Url);
            }
        }

        private async Task ProcessDataExportTaskAsync(BackgroundTask task)
        {
            var exportData = System.Text.Json.JsonSerializer.Deserialize<DataExportTaskData>(task.TaskData);
            if (exportData != null)
            {
                // Simulate data export processing
                await Task.Delay(1000); // Simulate work
                _logger.LogInformation("Data export completed for {ExportType}", exportData.ExportType);
            }
        }
    }

    // Task data models
    public class EmailTaskData
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; }
    }

    public class WebApiTaskData
    {
        public string Url { get; set; } = string.Empty;
        public Dictionary<string, string>? Headers { get; set; }
    }

    public class DataExportTaskData
    {
        public string ExportType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
    }
}
