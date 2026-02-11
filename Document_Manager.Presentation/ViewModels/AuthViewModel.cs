using Document_Manager.Application.DTOs.Auth;
using Document_Manager.Presentation.DTOs.Auth;
using Document_Manager.Presentation.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Document_Manager.Presentation.ViewModels
{
    public class AuthViewModel
    {
        private readonly AuthApiService _authService;
        private readonly ProtectedLocalStorage _storage;

        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string? Error { get; set; }

        public AuthViewModel(
            AuthApiService authService,
            ProtectedLocalStorage storage)
        {
            _authService = authService;
            _storage = storage;
        }

        public async Task<bool> LoginAsync()
        {
            Error = null;

            var result = await _authService.LoginAsync(new()
            {
                Email = Email,
                Password = Password
            });

            if (result is null)
            {
                Error = "Correo o contraseña incorrectos";
                return false;
            }

            // 🔥 AQUÍ SE GUARDA EL JWT
            await _storage.SetAsync("authToken", result.Token);

            return true;
        }
    }
}