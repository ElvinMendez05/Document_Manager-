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

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService,
            IEmailService emailService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _emailService = emailService;
        }

        // ✅ REGISTER
        public async Task RegisterAsync(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                throw new ApplicationException("Passwords do not match");

            var userExists = await _userManager.FindByEmailAsync(dto.Email);
            if (userExists != null)
                throw new ApplicationException("User already exists");

            var user = new ApplicationUser
            {
                FullName = dto.FullName,
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ",
                    result.Errors.Select(e => e.Description));

                throw new ApplicationException(errors);
            }

            // 🔥 Assign default role after successful creation
            if (!await _userManager.IsInRoleAsync(user, "User"))
            {
                await _userManager.AddToRoleAsync(user, "User");
            }
        }

        // ✅ LOGIN
        public async Task<string> LoginAsync(LoginRequest dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                throw new ApplicationException("Invalid credentials");

            var validPassword =
                await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!validPassword)
                throw new ApplicationException("Invalid credentials");

            // 🔥 Get real roles from Identity
            var roles = await _userManager.GetRolesAsync(user);

            // 🔥 Generate token with roles
            return _jwtTokenService.GenerateToken(user, roles);
        }

        // ✅ FORGOT PASSWORD
        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return; // Security measure

            var token =
                await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink =
                $"https://localhost/reset-password?token={Uri.EscapeDataString(token)}&email={dto.Email}";

            await _emailService.SendAsync(
                dto.Email,
                "Reset Password",
                $"Click here to reset your password:\n{resetLink}"
            );
        }

        // ✅ RESET PASSWORD
        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                throw new ApplicationException("User not found");

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
    //public class AuthService : IAuthService
    //{
    //    private readonly UserManager<ApplicationUser> _userManager;
    //    private readonly IJwtTokenService _jwtTokenService;
    //    private readonly IEmailService _emailService;

    //    public AuthService(UserManager<ApplicationUser> userManager, 
    //        IJwtTokenService jwtTokenService, 
    //        IEmailService emailService)
    //    {
    //        _userManager = userManager;
    //        _jwtTokenService = jwtTokenService;
    //        _emailService = emailService;
    //    }

    //    // REGISTER
    //    public async Task RegisterAsync(RegisterDto dto)
    //    {
    //        if (dto.Password != dto.ConfirmPassword)
    //            throw new ApplicationException("Las contraseñas no coinciden");

    //        var userExists = await _userManager.FindByEmailAsync(dto.Email);
    //        if (userExists != null)
    //            throw new ApplicationException("El usuario ya existe");

    //        var user = new ApplicationUser
    //        {
    //            FullName = dto.FullName,
    //            UserName = dto.Email,
    //            Email = dto.Email
    //        };

    //        var result = await _userManager.CreateAsync(user, dto.Password);

    //        if (!result.Succeeded)
    //        {
    //            var errors = string.Join(
    //                ", ",
    //                result.Errors.Select(e => e.Description));

    //            throw new ApplicationException(errors);
    //        }
    //    }

    //    // LOGIN 
    //    public async Task<string> LoginAsync(LoginRequest dto)
    //    {
    //        var user = await _userManager.FindByEmailAsync(dto.Email);

    //        if (user == null)
    //            throw new ApplicationException("Credenciales inválidas");

    //        var valid = await _userManager.CheckPasswordAsync(user, dto.Password);

    //        if (!valid)
    //            throw new ApplicationException("Credenciales inválidas");

    //        var roles = await _userManager.GetRolesAsync(user);

    //        return _jwtTokenService.GenerateToken(user, roles);
    //    }

    //    // FORGOT PASSWORD
    //    public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
    //    {
    //        var user = await _userManager.FindByEmailAsync(dto.Email);

    //        if (user == null)
    //            return; // seguridad

    //        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

    //        var resetLink =
    //            $"https://localhost:/reset-password?token={Uri.EscapeDataString(token)}&email={dto.Email}";

    //        await _emailService.SendAsync(
    //            dto.Email,
    //            "Restablecer contraseña",
    //            $"Haz clic aquí para restablecer tu contraseña:\n{resetLink}"
    //        );
    //    }

    //    // ResetPasswordAsync
    //    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    //    {
    //        var user = await _userManager.FindByEmailAsync(dto.Email);

    //        if (user == null)
    //            throw new ApplicationException("Usuario no encontrado");

    //        var result = await _userManager.ResetPasswordAsync(
    //            user,
    //            dto.Token,
    //            dto.NewPassword
    //            );

    //        if (!result.Succeeded)
    //        {
    //            var errors = string.Join(", ",
    //                result.Errors.Select(e => e.Description));

    //            throw new ApplicationException(errors);
    //        }
    //    }
    //}
}
