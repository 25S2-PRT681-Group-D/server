using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace AgroScan.API.Utilities
{
    public class EmailUtility : IEmailUtility
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailUtility> _logger;
        private readonly SmtpClient _smtpClient;

        public EmailUtility(IConfiguration configuration, ILogger<EmailUtility> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _smtpClient = new SmtpClient
            {
                Host = _configuration["Email:SmtpHost"] ?? "localhost",
                Port = int.Parse(_configuration["Email:SmtpPort"] ?? "587"),
                EnableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true"),
                Credentials = new NetworkCredential(
                    _configuration["Email:Username"],
                    _configuration["Email:Password"]
                )
            };
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            await SendEmailAsync(to, string.Empty, string.Empty, subject, body, isHtml);
        }

        public async Task SendEmailAsync(string to, string cc, string subject, string body, bool isHtml = false)
        {
            await SendEmailAsync(to, cc, string.Empty, subject, body, isHtml);
        }

        public async Task SendEmailAsync(string to, string cc, string bcc, string subject, string body, bool isHtml = false)
        {
            try
            {
                if (!await ValidateEmailAsync(to))
                {
                    throw new ArgumentException($"Invalid email address: {to}");
                }

                using var message = new MailMessage
                {
                    From = new MailAddress(_configuration["Email:FromAddress"] ?? "noreply@agroscan.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                message.To.Add(to);

                if (!string.IsNullOrEmpty(cc) && await ValidateEmailAsync(cc))
                {
                    message.CC.Add(cc);
                }

                if (!string.IsNullOrEmpty(bcc) && await ValidateEmailAsync(bcc))
                {
                    message.Bcc.Add(bcc);
                }

                await _smtpClient.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {To}", to);
                throw;
            }
        }

        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath, bool isHtml = false)
        {
            try
            {
                if (!await ValidateEmailAsync(to))
                {
                    throw new ArgumentException($"Invalid email address: {to}");
                }

                if (!File.Exists(attachmentPath))
                {
                    throw new FileNotFoundException($"Attachment file not found: {attachmentPath}");
                }

                using var message = new MailMessage
                {
                    From = new MailAddress(_configuration["Email:FromAddress"] ?? "noreply@agroscan.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                message.To.Add(to);
                message.Attachments.Add(new Attachment(attachmentPath));

                await _smtpClient.SendMailAsync(message);
                _logger.LogInformation("Email with attachment sent successfully to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email with attachment to {To}", to);
                throw;
            }
        }

        public async Task SendBulkEmailAsync(IEnumerable<string> recipients, string subject, string body, bool isHtml = false)
        {
            try
            {
                var validRecipients = new List<string>();
                foreach (var recipient in recipients)
                {
                    if (await ValidateEmailAsync(recipient))
                    {
                        validRecipients.Add(recipient);
                    }
                    else
                    {
                        _logger.LogWarning("Invalid email address skipped: {Email}", recipient);
                    }
                }

                if (!validRecipients.Any())
                {
                    throw new ArgumentException("No valid email addresses provided");
                }

                using var message = new MailMessage
                {
                    From = new MailAddress(_configuration["Email:FromAddress"] ?? "noreply@agroscan.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                foreach (var recipient in validRecipients)
                {
                    message.To.Add(recipient);
                }

                await _smtpClient.SendMailAsync(message);
                _logger.LogInformation("Bulk email sent successfully to {Count} recipients", validRecipients.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending bulk email");
                throw;
            }
        }

        public async Task<bool> ValidateEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return await Task.FromResult(emailRegex.IsMatch(email));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating email: {Email}", email);
                return false;
            }
        }

        public void Dispose()
        {
            _smtpClient?.Dispose();
        }
    }
}
