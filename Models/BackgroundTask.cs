namespace AgroScan.API.Models
{
    public class BackgroundTask
    {
        public string Id { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty;
        public string TaskData { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Queued, Processing, Completed, Failed, Cancelled
        public DateTime CreatedAt { get; set; }
        public DateTime ScheduledAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int Attempts { get; set; }
        public int MaxAttempts { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
