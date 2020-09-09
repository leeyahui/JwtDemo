using JwtDemo.Interface;
using JwtDemo.Models;

namespace JwtDemo.Services
{
    public class UserService : IUserService
    {
        public bool IsValid(LoginRequestDTO requestDto)
        {
            if (requestDto.Username == "admin" && requestDto.Password == "123")
            {
                return true;
            }
            return false;
        }
    }
}