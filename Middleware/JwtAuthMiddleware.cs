using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AzureFunction.Utility;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace AzureFunction.Middleware
{
    public class JwtAuthMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly string[] _ignoredPaths = ["/api/v1/GrantClientCredentialFunction"];
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                // 1. Get HttpRequestData from execution context
                var httpRequestData = await context.GetHttpRequestDataAsync();

                // If it's not an HTTP trigger (e.g. Timer, Queue trigger), skip JWT validation
                if (httpRequestData == null)
                {
                    await next(context);
                    return;
                }

                if (_ignoredPaths.Contains(httpRequestData.Url.AbsolutePath))
                {
                    await next(context);
                    return;
                }

                // 2. Extract Authorization header
                if (!httpRequestData.Headers.TryGetValues("Authorization", out var authHeaders))
                {
                    // await next(context);
                    await SetUnauthorizedResponse(context, httpRequestData, "Missing Authorization header.");
                    return;
                }

                string authHeader = authHeaders.FirstOrDefault() ?? "";
                if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    await SetUnauthorizedResponse(context, httpRequestData, "Invalid Authorization header format.");
                    return;
                }

                // 3. Read & Validate JWT format
                string token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();

                if (!handler.CanReadToken(token))
                {
                    await SetBadRequestResponse(context, httpRequestData, "Malformed JWT token.");
                    return;
                }

                // 4. Parse JWT and store items in FunctionContext.Items
                JwtSecurityToken jwtToken = handler.ReadJwtToken(token);

                string? userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "oid")?.Value;
                string? email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email" || c.Type == "preferred_username")?.Value;
                var roles = jwtToken.Claims.Where(c => c.Type == "roles" || c.Type == "role").Select(c => c.Value).ToList();

                var claims = new Claims
                {
                    Email = email ?? "default",
                    Roles = roles,
                    UserId = userId ?? "default",
                    ObjectId = jwtToken.Claims.FirstOrDefault(c => c.Type == "oid")?.Value ?? "default"
                };
                context.SetUser(claims);
         
                // Store user data in context so downstream Function Handlers can access it
                // context.Items["UserId"] = userId;
                // context.Items["Email"] = email;
                // context.Items["Roles"] = roles;
                // context.Items["JwtToken"] = jwtToken;

                // 5. Pass execution down the pipeline to the function handler
                await next(context);
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        private static async Task SetUnauthorizedResponse(FunctionContext context, HttpRequestData req, string message)
        {
            var response = req.CreateResponse(HttpStatusCode.Unauthorized);
            await response.WriteStringAsync(message);
            context.GetInvocationResult().Value = response;
        }

        private static async Task SetBadRequestResponse(FunctionContext context, HttpRequestData req, string message)
        {
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteStringAsync(message);
            context.GetInvocationResult().Value = response;
        }
    }
}