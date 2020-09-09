using JwtDemo.Models;

namespace JwtDemo.Interface
{
    public interface IAuthenticateService
    {
        bool IsAuthenticated(LoginRequestDTO request, out string token);
    }
}