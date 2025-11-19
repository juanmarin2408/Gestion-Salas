using AutoMapper;
using Domain;
using Domain.Enums;
using Infrastructure.Repositories;
using Moq;
using Services;

namespace ServicesTest;

public class AsesoriaServiceTests
{
    private readonly Mock<IAsesoriaRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly AsesoriaService _service;

    public AsesoriaServiceTests()
    {
        _service = new AsesoriaService(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAsesorias_OrdenaPorFechaYGeneraNumeroCorrelativo()
    {
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = "Laura",
            Apellido = "Torres",
            Documento = "ABC",
            Email = "laura@example.com",
            PasswordHash = "hash"
        };

        var older = new Asesoria
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            Usuario = usuario,
            Descripcion = "Antigua",
            FechaSolicitud = DateTime.UtcNow.AddDays(-2)
        };

        var newer = new Asesoria
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            Usuario = usuario,
            Descripcion = "Reciente",
            FechaSolicitud = DateTime.UtcNow.AddDays(-1)
        };

        _repositoryMock.Setup(r => r.GetAsesorias())
            .ReturnsAsync(new List<Asesoria> { older, newer });

        var resultado = await _service.GetAsesorias();

        Assert.Equal(2, resultado.Count);
        Assert.Equal(newer.Id, resultado[0].Id);
        Assert.Equal(1, resultado[0].NumeroTicket);
        Assert.Equal(older.Id, resultado[1].Id);
        Assert.Equal(2, resultado[1].NumeroTicket);
    }

    [Fact]
    public async Task AceptarAsesoria_Pendiente_ActualizaEstadoYFechas()
    {
        var asesoria = new Asesoria
        {
            Id = Guid.NewGuid(),
            UsuarioId = Guid.NewGuid(),
            Usuario = BuildUsuario("Mario"),
            Descripcion = "Soporte",
            Estado = EstadoAsesoria.Pendiente
        };

        var atendidoPor = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetAsesoria(asesoria.Id))
            .ReturnsAsync(asesoria);

        await _service.AceptarAsesoria(asesoria.Id, atendidoPor);

        Assert.Equal(EstadoAsesoria.EnProceso, asesoria.Estado);
        Assert.Equal(atendidoPor, asesoria.AtendidoPorId);
        Assert.NotNull(asesoria.FechaInicio);
        _repositoryMock.Verify(r => r.Update(asesoria), Times.Once);
    }

    [Fact]
    public async Task RechazarAsesoria_SinMotivo_LanzaArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.RechazarAsesoria(Guid.NewGuid(), Guid.NewGuid(), "   "));
    }

    [Fact]
    public async Task RechazarAsesoria_Pendiente_RecortaMotivoYActualizaEstado()
    {
        var asesoria = new Asesoria
        {
            Id = Guid.NewGuid(),
            UsuarioId = Guid.NewGuid(),
            Usuario = BuildUsuario("Claudia"),
            Descripcion = "Solicitud",
            Estado = EstadoAsesoria.Pendiente
        };

        _repositoryMock.Setup(r => r.GetAsesoria(asesoria.Id))
            .ReturnsAsync(asesoria);

        await _service.RechazarAsesoria(asesoria.Id, Guid.NewGuid(), "  Falta info  ");

        Assert.Equal(EstadoAsesoria.Rechazado, asesoria.Estado);
        Assert.Equal("Falta info", asesoria.MotivoRechazo);
        Assert.NotNull(asesoria.FechaResolucion);
        _repositoryMock.Verify(r => r.Update(asesoria), Times.Once);
    }

    private static Usuario BuildUsuario(string nombre) => new()
    {
        Id = Guid.NewGuid(),
        Nombre = nombre,
        Apellido = "Test",
        Documento = "DOC",
        Email = $"{nombre.ToLower()}@example.com",
        PasswordHash = "hash"
    };
}

