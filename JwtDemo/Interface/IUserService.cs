using JwtDemo.Models;

namespace JwtDemo.Interface
{
    public interface IUserService
    {
        bool IsValid(LoginRequestDTO requestDto);
    }
}