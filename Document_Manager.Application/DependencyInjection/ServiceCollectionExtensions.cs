using Document_Manager.Application.Interfaces;
using Document_Manager.Application.Interfaces.Security;
using Document_Manager.Application.Services;
using Document_Manager.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Document_Manager.Application.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            return services;
        }
    }
}
