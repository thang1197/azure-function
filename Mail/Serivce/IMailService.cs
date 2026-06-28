using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunction.Mail.Serivce
{
    public interface IMailService
    {
        public Task<bool> SendEmailAsync(string to, string subject, string body);
    }
}