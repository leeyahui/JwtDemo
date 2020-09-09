using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtDemo.Interface;
using JwtDemo.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JwtDemo.Services
{
    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly TokenManagement _tokenManagement;
        private readonly IUserService _userService;

        public TokenAuthenticationService(IUserService userService, IOptions<TokenManagement> tokenManagement)
        {
            _userService = userService;
            _tokenManagement = tokenManagement.Value;
        }

        public bool IsAuthenticated(LoginRequestDTO request, out string token)
        {
            token = string.Empty;
            if (!_userService.IsValid(request)) return false;

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.Username),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(
                issuer: _tokenManagement.Issuer,
                audience: _tokenManagement.Audience,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddSeconds(_tokenManagement.AccessExpiration),
                claims: claims,
                signingCredentials: credentials);

            token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return true;
        }
    }
}