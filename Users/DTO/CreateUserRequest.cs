using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Azure_Function.Users.DTO
{
    public class CreateUserRequest
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }

        public required string Password { get; set; }
    }
}