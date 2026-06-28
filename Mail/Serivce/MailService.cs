using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AzureFunction.Mail.Serivce
{
    public class MailService(ILogger<MailService> logger) : IMailService
    {
        private readonly ILogger<MailService> _logger = logger;
        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                _logger.LogInformation("Sending email to {To} with subject {Subject}", to, subject);
                // Email sending logic would go here
                await Task.Delay(500); // Simulate async operation
                _logger.LogInformation("Email sent to {To} successfully", to);
                return true;
            }
            catch (System.Exception e)
            {
                _logger.LogError("Failed to send email to {To}", to);
                throw new System.Exception("Failed to send email: " + e.Message);
            }
        }
    }
}