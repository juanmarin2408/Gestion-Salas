using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Services
{
    public static class Dependencyinjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Servicios
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISalaService, SalaService>();
            services.AddScoped<IEquipoService, EquipoService>();

            // Hasher para contraseñas de Usuario
            services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

            return services;
        }
    }
}
