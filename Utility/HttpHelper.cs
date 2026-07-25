using System.ComponentModel.DataAnnotations;
using Azure.Core;
using Azure.Identity;
using AzureFunction.Configuration.App;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AzureFunction.Utility
{
    public class HttpHelper(ApiManagementSettings apiManagementsettings, AzureAdSettings azureAdSettings, ClientSecretCredential clientSecretCredential)
    {
        private readonly ApiManagementSettings _apiManagementsettings = apiManagementsettings;
        private readonly ClientSecretCredential _clientSecretCredential = clientSecretCredential;
        private readonly AzureAdSettings _azureAdSettings = azureAdSettings;
        private readonly HttpClient _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(10),
            BaseAddress = new Uri(apiManagementsettings.Url)
        };
        public static IActionResult GenerateResponse<T>(T data, int statusCode = StatusCodes.Status200OK)
        {
            try
            {
                var response = new
                {
                    status = "success",
                    code = statusCode,
                    data = data
                };
                var result = new ObjectResult(response)
                {
                    StatusCode = statusCode
                };

                result.ContentTypes.Add("application/json");
                return result;
            }
            catch (Exception ex)
            {
                var response = new
                {
                    status = "error",
                    code = StatusCodes.Status500InternalServerError,
                    error = ex.Message
                };
                var result = new ObjectResult(response)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                return result;
            }
        }

        public static IActionResult GenerateErrorResponse(string errorMessage, int statusCode = StatusCodes.Status400BadRequest)
        {
            var response = new
            {
                status = "error",
                code = statusCode,
                error = errorMessage
            };
            var result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };
            return result;
        }

        public static IEnumerable<string?>? ValidateRequestBody<T>(T requestBody)
        {
            if (requestBody is null)
            {
                return null;
            }

            var validationContext = new ValidationContext(requestBody);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(
                requestBody,
                validationContext,
                validationResults,
                validateAllProperties: true);

            if (!isValid)
            {
                return validationResults.Select(v => v.ErrorMessage);
            }
            return null;
        }

        public HttpClient GetHttpClient()
        {
            return _httpClient;
        }

        public async Task<string> GetAdToken()
        {
            try
            {
                var token = await _clientSecretCredential.GetTokenAsync(new TokenRequestContext([_azureAdSettings.Scope]));
                return token.Token;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}