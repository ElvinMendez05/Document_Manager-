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
        private readonly IEmailService _emailService;

        public AuthService(UserManager<ApplicationUser> userManager, 
            IJwtTokenService jwtTokenService, 
            IEmailService emailService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _emailService = emailService;
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
                return; // seguridad

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink =
                $"https://tu-frontend/reset-password?token={Uri.EscapeDataString(token)}&email={dto.Email}";

            await _emailService.SendAsync(
                dto.Email,
                "Restablecer contraseña",
                $"Haz clic aquí para restablecer tu contraseña:\n{resetLink}"
            );
        }

        // ResetPasswordAsync
        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                throw new ApplicationException("Usuario no encontrado");

            var result = await _userManager.ResetPasswordAsync(
                user,
                dto.Token,
                dto.NewPassword
                );

            if (!result.Succeeded)
            {
                var errors = string.Join(", ",
                    result.Errors.Select(e => e.Description));

                throw new ApplicationException(errors);
            }
        }
    }
}
