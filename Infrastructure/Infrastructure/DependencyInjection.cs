using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            var c = configuration.GetConnectionString("DefaultConnection");

            // Repositorios
            services.AddScoped<IFarmRepository, FarmRepository>();
            services.AddScoped<IUserRepository, UserRepository>(); // 👈 ESTE DEBE ESTAR

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(c);
            });

            return services;
        }
    }
}
