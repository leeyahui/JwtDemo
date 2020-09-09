using JwtDemo.Interface;
using JwtDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticateService _authenticateService;

        public AuthenticationController(IAuthenticateService authenticateService)
        {
            _authenticateService = authenticateService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("requestToken")]
        public ActionResult RequestToken([FromBody] LoginRequestDTO request)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid Request");

            if (_authenticateService.IsAuthenticated(request, out var token))
            {
                return Ok(new
                {
                    access_token = token,
                    token_type = "Bearer",
                });
            }

            return BadRequest("Invalid Request");
        }
    }
}