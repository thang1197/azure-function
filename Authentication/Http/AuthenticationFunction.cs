using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AzureFunction.Authentication.DTO;
using AzureFunction.Authentication.Service;
using AzureFunction.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunction.Authentication.Http
{
    public class AuthenticationFunction(IAuthenticationService authenticationService, ILogger<AuthenticationFunction> logger)
    {
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
        private readonly ILogger<AuthenticationFunction> _logger = logger;
        private readonly IAuthenticationService _authenticationService = authenticationService;
        [Function("AuthenticationFunction")]
        public async Task<IActionResult> Authenticate([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            // Implementation for the authentication function
            try
            {
                var loginRequest = await JsonSerializer.DeserializeAsync<LoginRequest>(req.Body, _jsonOptions);

                if (loginRequest is null)
                {
                    return HttpHelper.GenerateErrorResponse("Invalid request body", StatusCodes.Status400BadRequest);
                }

                var token = _authenticationService.GenerateToken(loginRequest.Username, loginRequest.Password);
                return HttpHelper.GenerateResponse(token, StatusCodes.Status201Created);
            }
            catch (System.Exception ex)
            {

                return HttpHelper.GenerateErrorResponse(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        [Function("GrantClientCredentialFunction")]
        public async Task<IActionResult> GrantClientCredential([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            // Implementation for the authentication function
            try
            {
                // var loginRequest = await JsonSerializer.DeserializeAsync<LoginRequest>(req.Body, _jsonOptions);

                // if (loginRequest is null)
                // {
                //     return HttpHelper.GenerateErrorResponse("Invalid request body", StatusCodes.Status400BadRequest);
                // }

                var token = _authenticationService.GrantClientCredentials();
                return HttpHelper.GenerateResponse(token, StatusCodes.Status201Created);
            }
            catch (System.Exception ex)
            {
                return HttpHelper.GenerateErrorResponse(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }
    }
}