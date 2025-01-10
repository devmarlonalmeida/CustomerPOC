using Application.DTOs;
using Application.Services;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientesPOCApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public LoginController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Email != "test@example.com")
            {
                return BadRequest(new UserNotFoundException());
            }

            var token = _tokenService.GenerateToken(request.Email);
            return Ok(new { Token = token });
        }
    }
}
