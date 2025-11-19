using Domain;
using Domain.Enums;
using Infrastructure.Repositories;

namespace InfrastructureTest;

public class SalaRepositoryTests
{
    [Fact]
    public async Task GetSalas_IncluyeUsuarioYEquipos()
    {
        using var context = InMemoryDbContextFactory.CreateContext();

        var usuario = BuildUsuario("Coordinador");
        var sala = new Sala
        {
            Id = Guid.NewGuid(),
            Numero = "LAB-101",
            Capacidad = 20,
            Ubicacion = "Bloque A",
            UsuarioId = usuario.Id,
            Usuario = usuario,
            Estado = EstadoSala.Activa
        };

        var equipo = new Equipo
        {
            Id = Guid.NewGuid(),
            SalaId = sala.Id,
            Sala = sala,
            Estado = EstadoEquipo.Disponible
        };

        sala.Equipos.Add(equipo);

        context.Usuarios.Add(usuario);
        context.Salas.Add(sala);
        context.Equipos.Add(equipo);
        await context.SaveChangesAsync();

        var repository = new SalaRepository(context);

        var result = await repository.GetSalas();
        var storedSala = Assert.Single(result);

        Assert.NotNull(storedSala.Usuario);
        Assert.Equal(usuario.Email, storedSala.Usuario!.Email);
        Assert.Single(storedSala.Equipos);
        Assert.Equal(equipo.Id, storedSala.Equipos.First().Id);
    }

    [Fact]
    public async Task GetSalasByEstado_FiltraPorEstado()
    {
        using var context = InMemoryDbContextFactory.CreateContext();

        var usuario = BuildUsuario("Responsable");
        var activa = new Sala
        {
            Id = Guid.NewGuid(),
            Numero = "A-1",
            Capacidad = 10,
            Ubicacion = "Primer piso",
            UsuarioId = usuario.Id,
            Usuario = usuario,
            Estado = EstadoSala.Activa
        };

        var inactiva = new Sala
        {
            Id = Guid.NewGuid(),
            Numero = "B-2",
            Capacidad = 12,
            Ubicacion = "Segundo piso",
            Estado = EstadoSala.Inactiva
        };

        context.Usuarios.Add(usuario);
        context.Salas.AddRange(activa, inactiva);
        await context.SaveChangesAsync();

        var repository = new SalaRepository(context);

        var activas = await repository.GetSalasByEstado(EstadoSala.Activa);

        var salaActiva = Assert.Single(activas);
        Assert.Equal(activa.Numero, salaActiva.Numero);
        Assert.Equal(EstadoSala.Activa, salaActiva.Estado);
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
}

