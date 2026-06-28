using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AzureFunction.Mail.DTO;
using AzureFunction.Mail.Serivce;
using AzureFunction.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace AzureFunction.Mail.Http
{
    public class MailFunction(IMailService mailService)
    {
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
        private readonly IMailService _mailService = mailService;
        [Function("SendEmailFunction")]
        public async Task<IActionResult> SendEmailAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request)
        {
            // Implementation for sending email
            try
            {
                var mailRequest = await JsonSerializer.DeserializeAsync<MailRequest>(request.Body, _jsonOptions);
                if (mailRequest is null)
                {
                    return HttpHelper.GenerateErrorResponse("Invalid request body", StatusCodes.Status400BadRequest);
                }

                var result = await _mailService.SendEmailAsync(mailRequest.To, mailRequest.Subject, mailRequest.Body);
                if (result)
                {
                    return HttpHelper.GenerateResponse("Email sent successfully.");
                }
                else
                {
                    return HttpHelper.GenerateErrorResponse("Failed to send email", StatusCodes.Status500InternalServerError);
                }
            }
            catch (System.Exception)
            {
                return HttpHelper.GenerateErrorResponse("An error occurred while processing the request", StatusCodes.Status500InternalServerError);
            }
        }
    }
}