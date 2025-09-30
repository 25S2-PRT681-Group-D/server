namespace AgroScan.API.Utilities
{
    public interface IEmailUtility
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);
        Task SendEmailAsync(string to, string cc, string subject, string body, bool isHtml = false);
        Task SendEmailAsync(string to, string cc, string bcc, string subject, string body, bool isHtml = false);
        Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath, bool isHtml = false);
        Task SendBulkEmailAsync(IEnumerable<string> recipients, string subject, string body, bool isHtml = false);
        Task<bool> ValidateEmailAsync(string email);
    }
}
