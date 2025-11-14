using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Services
{
    public static class Dependencyinjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Servicios existentes
            services.AddTransient<IFarmService, FarmService>();

            // ✅ Servicio de usuarios: interfaz -> implementación
            services.AddScoped<IUserService, UserService>();

            // Hasher para contraseñas de Usuario
            services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

            return services;
        }
    }
}
