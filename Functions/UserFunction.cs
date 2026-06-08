using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure_Function.Services;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using Azure_Function.Users.DTO;
using Azure.Storage.Queues;

namespace Azure_Function.Functions;

public class UserFunction(ILogger<UserFunction> logger, IUserService userService, QueueClient queueClient)
{
    // Cache the options globally for this class
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly QueueClient _queueClient = queueClient;

    private readonly ILogger<UserFunction> _logger = logger;
    private readonly IUserService _userService = userService;

    // [Function("GetUserFunction")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "users/{id}")]
        HttpRequest req, string id)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        var userId = id;

        if (string.IsNullOrEmpty(userId))
        {
            return new BadRequestResult();
        }
        var user = _userService.GetUserById(userId);

        var data = new
        {
            status = "success",
            code = 200,
            data = user
        };
        var result = new ObjectResult(data)
        {
            StatusCode = StatusCodes.Status200OK
        };

        result.ContentTypes.Add("application/json");
        return result;
    }

    // [Function("CreateUserFunction")]
    async public Task<IActionResult> CreateUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users")]
        HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var user = await JsonSerializer.DeserializeAsync<CreateUserRequest>(
            req.Body, _jsonOptions);

        if (user == null)
        {
            return new BadRequestResult();
        }
        var createdUser = _userService.CreateUser(user);

        var data = new
        {
            status = "success",
            code = 201,
            data = createdUser
        };
        var result = new ObjectResult(data)
        {
            StatusCode = StatusCodes.Status201Created
        };

        result.ContentTypes.Add("application/json");

        // Send a message to the queue
        var queueMessage = JsonSerializer.Serialize(new
        {
            UserId = createdUser.Id,
            UserName = createdUser.UserName,
            Email = createdUser.Email
        }, _jsonOptions);
        
        await _queueClient.SendMessageAsync(queueMessage);
        return result;
    }
}
