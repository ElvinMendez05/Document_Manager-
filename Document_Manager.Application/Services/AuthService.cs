using Document_Manager.Application.DTOs.Auth;
using Document_Manager.Application.Interfaces;
using Document_Manager.Application.Interfaces.Security;
using Document_Manager.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Document_Manager.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        // REGISTER
        public async Task RegisterAsync(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                throw new ApplicationException("Las contraseñas no coinciden");

            var userExists = await _userManager.FindByEmailAsync(dto.Email);
            if (userExists != null)
                throw new ApplicationException("El usuario ya existe");

            var user = new ApplicationUser
            {
                FullName = dto.FullName,
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new ApplicationException(errors);
            }
        }

        // LOGIN 
        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                throw new ApplicationException("Credenciales inválidas");

            var valid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!valid)
                throw new ApplicationException("Credenciales inválidas");

            return _jwtTokenService.GenerateToken(user);
        }

        // FORGOT PASSWORD
        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        }
    }
}
