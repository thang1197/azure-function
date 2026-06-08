using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure_Function.Services;
using Azure_Function.Users.DTO;

namespace Azure_Function.Users
{
    public class UserService : IUserService
    {
        public string GetUserById(string id)
        {
            return $"User with ID: {id}";
        }

        public UserEntity CreateUser(CreateUserRequest dto)
        {
            return new UserEntity
            {
                Id = Guid.NewGuid().ToString(),
                UserName = dto.UserName,
                Email = dto.Email,
                Password = dto.Password
            };
        }
    }
}