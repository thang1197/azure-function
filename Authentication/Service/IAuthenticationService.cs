using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunction.Authentication.Service
{
    public interface IAuthenticationService
    {
        public string GenerateToken(string username, string password);

        public string GrantClientCredentials();
    }
}