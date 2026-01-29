using Document_Manager.Application.DTOs.Auth;
using Document_Manager.Presentation.DTOs.Auth;
using Document_Manager.Presentation.Services;
using Microsoft.AspNetCore.Identity.Data;

namespace Document_Manager.Presentation.ViewModels
{
    public class AuthViewModel
    {
        private readonly AuthApiService _authService;

        public string Email { get; set; } = "";
        public string Password { get; set; } = "";

        public string? Error { get; set; }

        public AuthViewModel(AuthApiService authService)
        {
            _authService = authService;
        }

        public async Task<bool> LoginAsync()
        {
            Error = null;

            var request = new LoginRequestDto
            {
                Email = Email,
                Password = Password
            };

            var result = await _authService.LoginAsync(request);

            if (result is null)
            {
                Error = "Correo o contraseña incorrectos";
                return false;
            }

            // Aquí luego guardaremos el JWT
            return true;

            //    private readonly AuthApiService _authApi;
            //    private readonly TokenStorageService _tokenStorage;

            //    public AuthViewModel(
            //        AuthApiService authApi,
            //        TokenStorageService tokenStorage)
            //    {
            //        _authApi = authApi;
            //        _tokenStorage = tokenStorage;
            //    }

            //    // Binding
            //    public string Email { get; set; } = "";
            //    public string Password { get; set; } = "";

            //    public string? ErrorMessage { get; private set; }

            //    public async Task<bool> LoginAsync()
            //    {
            //        ErrorMessage = null;

            //        var response = await _authApi.LoginAsync(new LoginRequestDto
            //        {
            //            Email = Email,
            //            Password = Password
            //        });

            //        if (response == null)
            //        {
            //            ErrorMessage = "Credenciales inválidas";
            //            return false;
            //        }

            //        _tokenStorage.SaveToken(response.Token);
            //        return true;
            //    }
            //}
        }
    }
}