using Document_Manager.Application.DTOs.Auth;
using Document_Manager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Document_Manager.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) {
        
           _authService = authService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto) 
        {
            try
            {
                await _authService.RegisterAsync(dto);
                return Ok(new
                {
                    message = "Usuario registrado correctamente"
                });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _authService.LoginAsync(dto);

                return Ok(new
                {
                    token
                });
            }
            catch (ApplicationException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }

        // POST: api/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            await _authService.ForgotPasswordAsync(dto);
            return Ok(new { message = "Si el correo existe, se enviará un enlace de recuperación" });
        }
    }
}
