using Domain;
using Domain.Enums;
using Infrastructure.Repositories;

namespace InfrastructureTest;

public class SolicitudPrestamoRepositoryTests
{
    [Fact]
    public async Task GetSolicitudes_OrdenaPorFechaDescendente()
    {
        using var context = InMemoryDbContextFactory.CreateContext();

        var usuario = BuildUsuario("Mario");
        var sala = BuildSala("LAB-03");

        var antigua = new SolicitudPrestamo
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            Usuario = usuario,
            SalaId = sala.Id,
            Sala = sala,
            TiempoEstimado = 2,
            FechaSolicitud = DateTime.UtcNow.AddDays(-2),
            Estado = EstadoSolicitud.Pendiente
        };

        var reciente = new SolicitudPrestamo
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            Usuario = usuario,
            SalaId = sala.Id,
            Sala = sala,
            TiempoEstimado = 4,
            FechaSolicitud = DateTime.UtcNow,
            Estado = EstadoSolicitud.Aprobada,
            EquipoId = Guid.NewGuid()
        };

        context.Usuarios.Add(usuario);
        context.Salas.Add(sala);
        context.SolicitudesPrestamo.AddRange(antigua, reciente);
        await context.SaveChangesAsync();

        var repository = new SolicitudPrestamoRepository(context);

        var solicitudes = await repository.GetSolicitudes();

        Assert.Equal(new[] { reciente.Id, antigua.Id }, solicitudes.Select(s => s.Id));
        Assert.All(solicitudes, solicitud =>
        {
            Assert.NotNull(solicitud.Usuario);
            Assert.NotNull(solicitud.Sala);
        });
    }

    [Fact]
    public async Task GetSolicitudesByEstado_FiltraPendientes()
    {
        using var context = InMemoryDbContextFactory.CreateContext();

        var usuario = BuildUsuario("Laura");
        var sala = BuildSala("LAB-04");

        var pendiente = new SolicitudPrestamo
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            Usuario = usuario,
            SalaId = sala.Id,
            Sala = sala,
            TiempoEstimado = 3,
            FechaSolicitud = DateTime.UtcNow,
            Estado = EstadoSolicitud.Pendiente
        };

        var aprobada = new SolicitudPrestamo
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            Usuario = usuario,
            SalaId = sala.Id,
            Sala = sala,
            TiempoEstimado = 5,
            FechaSolicitud = DateTime.UtcNow,
            Estado = EstadoSolicitud.Aprobada
        };

        context.Usuarios.Add(usuario);
        context.Salas.Add(sala);
        context.SolicitudesPrestamo.AddRange(pendiente, aprobada);
        await context.SaveChangesAsync();

        var repository = new SolicitudPrestamoRepository(context);

        var pendientes = await repository.GetSolicitudesByEstado(EstadoSolicitud.Pendiente);

        var unica = Assert.Single(pendientes);
        Assert.Equal(pendiente.Id, unica.Id);
        Assert.Equal(EstadoSolicitud.Pendiente, unica.Estado);
    }

    private static Usuario BuildUsuario(string nombre) => new()
    {
        Id = Guid.NewGuid(),
        Nombre = nombre,
        Apellido = "Tester",
        Documento = Guid.NewGuid().ToString("N"),
        Email = $"{nombre.ToLower()}@example.com",
        PasswordHash = "hash"
    };

    private static Sala BuildSala(string numero) => new()
    {
        Id = Guid.NewGuid(),
        Numero = numero,
        Capacidad = 30,
        Ubicacion = "Edificio Principal"
    };
}

