using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using AzureFunction.Tasks.DTO;
using AzureFunction.Utility;
using Microsoft.Extensions.Logging;

namespace AzureFunction.Tasks.Service
{
    public class TaskService(HttpHelper httpHelper, ILogger<TaskService> logger) : ITaskService
    {
        private readonly HttpHelper _httpHelper = httpHelper;
        private ILogger<TaskService> _logger = logger;
        public async Task<Entity.Task> CreateTask(CreateTaskRequest request)
        {
            try
            {
                var token = await _httpHelper.GetAdToken();
                var httpClient = _httpHelper.GetHttpClient();
                HttpRequestMessage httpRequestMessage = new()
                {
                    Content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json"),
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("task-api/api/task", UriKind.Relative),
                    Headers = { { "Authorization", "Bearer " + token } }
                };

                var response = await httpClient.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<Entity.Task>() ?? new Entity.Task();
                    // Do something with result
                    return result;
                }
                else
                {
                    // Read the error message from the API to see what went wrong
                    string errorResponseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"API Error: {response.StatusCode} - {errorResponseBody}");
                    throw new ValidationException(errorResponseBody);
                }
                // _logger.LogInformation("Response: {Response}", response);
                // return response;
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public Task<Entity.Task> UpdateTaskById(int id, UpdateTaskRequest request)
        {
           throw new NotImplementedException();
        }
    }
}