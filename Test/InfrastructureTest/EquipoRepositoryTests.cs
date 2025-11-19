using Domain;
using Domain.Enums;
using Infrastructure.Repositories;

namespace InfrastructureTest;

public class EquipoRepositoryTests
{
    [Fact]
    public async Task GetEquiposByEstado_RetornaSoloEquiposConEstado()
    {
        using var context = InMemoryDbContextFactory.CreateContext();

        var sala = new Sala
        {
            Id = Guid.NewGuid(),
            Numero = "LAB-01",
            Capacidad = 25,
            Ubicacion = "Edificio Central"
        };

        var disponible = new Equipo
        {
            Id = Guid.NewGuid(),
            SalaId = sala.Id,
            Sala = sala,
            Estado = EstadoEquipo.Disponible
        };

        var asignado = new Equipo
        {
            Id = Guid.NewGuid(),
            SalaId = sala.Id,
            Sala = sala,
            Estado = EstadoEquipo.Asignado
        };

        context.Salas.Add(sala);
        context.Equipos.AddRange(disponible, asignado);
        await context.SaveChangesAsync();

        var repository = new EquipoRepository(context);

        var disponibles = await repository.GetEquiposByEstado(EstadoEquipo.Disponible);

        var equipo = Assert.Single(disponibles);
        Assert.Equal(disponible.Id, equipo.Id);
        Assert.NotNull(equipo.Sala);
    }

    [Fact]
    public async Task GetEquiposBySalaId_IncluyeAsignadoA()
    {
        using var context = InMemoryDbContextFactory.CreateContext();

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = "Laura",
            Apellido = "Campos",
            Documento = "DOC123",
            Email = "laura@example.com",
            PasswordHash = "hash"
        };

        var sala = new Sala
        {
            Id = Guid.NewGuid(),
            Numero = "LAB-02",
            Capacidad = 15,
            Ubicacion = "Bloque B"
        };

        var equipo = new Equipo
        {
            Id = Guid.NewGuid(),
            SalaId = sala.Id,
            Sala = sala,
            Estado = EstadoEquipo.Asignado,
            AsignadoAId = usuario.Id,
            AsignadoA = usuario
        };

        context.Usuarios.Add(usuario);
        context.Salas.Add(sala);
        context.Equipos.Add(equipo);
        await context.SaveChangesAsync();

        var repository = new EquipoRepository(context);

        var equipos = await repository.GetEquiposBySalaId(sala.Id);

        var stored = Assert.Single(equipos);
        Assert.NotNull(stored.AsignadoA);
        Assert.Equal(usuario.Email, stored.AsignadoA!.Email);
    }
}

