using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;

namespace AzureFunction.Utility
{
    public static class FunctionContextExtensions
    {
        private const string UserKey = "User";

        public static void SetUser(this FunctionContext context, Claims user)
        {
            context.Items[UserKey] = user;
        }

        public static Claims? GetUser(this FunctionContext context)
        {
            return context.Items.TryGetValue(UserKey, out var value)
                ? value as Claims
                : null;
        }
    }

    public class Claims
    {
        public string ObjectId { get; init; } = default!;
        public string Email { get; init; } = default!;
        public List<string> Roles { get; init; } = default!;
        public string UserId { get; init; } = default!;
    }
}