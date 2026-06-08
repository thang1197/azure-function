using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure_Function.Users;
using Azure_Function.Users.DTO;

namespace Azure_Function.Services
{
    public interface IUserService
    {
        string GetUserById(string id);
        UserEntity CreateUser(CreateUserRequest dto);
    }
}