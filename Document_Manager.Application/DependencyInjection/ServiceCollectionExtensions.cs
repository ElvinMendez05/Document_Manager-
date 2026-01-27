using Document_Manager.Application.Interfaces;
using Document_Manager.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Document_Manager.Application.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            
            services.AddScoped<IDocumentService, DocumentService>();

            return services;
        }
    }
}
