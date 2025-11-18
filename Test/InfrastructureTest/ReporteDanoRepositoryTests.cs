using Domain;
using Domain.Enums;
using Infrastructure.Repositories;

namespace InfrastructureTest;

public class ReporteDanoRepositoryTests
{
    [Fact]
    public async Task GetReportesByEstado_IncluyeSalaYEquipo()
    {
        using var context = InMemoryDbContextFactory.CreateContext();

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = "Sofía",
            Apellido = "Peralta",
            Documento = "ABC123",
            Email = "sofia@example.com",
            PasswordHash = "hash"
        };

        var sala = new Sala
        {
            Id = Guid.NewGuid(),
            Numero = "LAB-05",
            Capacidad = 18,
            Ubicacion = "Bloque C"
        };

        var equipo = new Equipo
        {
            Id = Guid.NewGuid(),
            SalaId = sala.Id,
            Sala = sala,
            Estado = EstadoEquipo.Disponible
        };

        var pendiente = new ReporteDano
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            Usuario = usuario,
            Tipo = TipoReporte.Equipo,
            EquipoId = equipo.Id,
            Equipo = equipo,
            SalaId = sala.Id,
            Sala = sala,
            Descripcion = "Sin batería",
            Estado = EstadoReporte.Pendiente,
            Prioridad = PrioridadReporte.Alta,
            FechaReporte = DateTime.UtcNow
        };

        var resuelto = new ReporteDano
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            Usuario = usuario,
            Tipo = TipoReporte.Sala,
            SalaId = sala.Id,
            Sala = sala,
            Descripcion = "Cable suelto",
            Estado = EstadoReporte.Resuelto,
            Prioridad = PrioridadReporte.Media,
            FechaReporte = DateTime.UtcNow.AddHours(-1),
            FechaResolucion = DateTime.UtcNow
        };

        context.Usuarios.Add(usuario);
        context.Salas.Add(sala);
        context.Equipos.Add(equipo);
        context.ReportesDanos.AddRange(pendiente, resuelto);
        await context.SaveChangesAsync();

        var repository = new ReporteDanoRepository(context);

        var pendientes = await repository.GetReportesByEstado(EstadoReporte.Pendiente);

        var reporte = Assert.Single(pendientes);
        Assert.Equal(pendiente.Id, reporte.Id);
        Assert.NotNull(reporte.Equipo);
        Assert.NotNull(reporte.Sala);
        Assert.Equal(EstadoReporte.Pendiente, reporte.Estado);
    }

    [Fact]
    public async Task GetReportes_RetornaOrdenadosPorFecha()
    {
        using var context = InMemoryDbContextFactory.CreateContext();

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = "Diego",
            Apellido = "Mora",
            Documento = "XYZ789",
            Email = "diego@example.com",
            PasswordHash = "hash"
        };

        var sala = new Sala
        {
            Id = Guid.NewGuid(),
            Numero = "LAB-06",
            Capacidad = 12,
            Ubicacion = "Bloque D"
        };

        var masReciente = new ReporteDano
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            Usuario = usuario,
            Tipo = TipoReporte.Sala,
            SalaId = sala.Id,
            Sala = sala,
            Descripcion = "Fuga de agua",
            Estado = EstadoReporte.EnRevision,
            Prioridad = PrioridadReporte.Urgente,
            FechaReporte = DateTime.UtcNow
        };

        var masAntiguo = new ReporteDano
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            Usuario = usuario,
            Tipo = TipoReporte.Sala,
            SalaId = sala.Id,
            Sala = sala,
            Descripcion = "Pintura dañada",
            Estado = EstadoReporte.Pendiente,
            Prioridad = PrioridadReporte.Baja,
            FechaReporte = DateTime.UtcNow.AddDays(-1)
        };

        context.Usuarios.Add(usuario);
        context.Salas.Add(sala);
        context.ReportesDanos.AddRange(masAntiguo, masReciente);
        await context.SaveChangesAsync();

        var repository = new ReporteDanoRepository(context);

        var reportes = await repository.GetReportes();

        Assert.Equal(new[] { masReciente.Id, masAntiguo.Id }, reportes.Select(r => r.Id));
    }
}

