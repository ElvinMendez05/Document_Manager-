using Document_Manager.Application.Interfaces;
using Document_Manager.Application.Services;
using Document_Manager.Domain.Interfaces;
using Document_Manager.Infrastructure.Persistence;
using Document_Manager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Document_Manager.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IFileStorageService, FileStorageService>();

            return services;
        }
    }
}
