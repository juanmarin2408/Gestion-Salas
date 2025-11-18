using AutoMapper;
using Domain;
using Domain.Enums;
using Infrastructure.Repositories;
using Moq;
using Services;

namespace ServicesTest;

public class SolicitudPrestamoServiceTests
{
    private readonly Mock<ISolicitudPrestamoRepository> _solicitudRepository = new();
    private readonly Mock<IEquipoRepository> _equipoRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly SolicitudPrestamoService _service;

    public SolicitudPrestamoServiceTests()
    {
        _service = new SolicitudPrestamoService(_solicitudRepository.Object, _equipoRepository.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetSolicitudesUrgentesCount_CuentaSolicitudesConMasDe24HorasPendientes()
    {
        var solicitudes = new List<SolicitudPrestamo>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UsuarioId = Guid.NewGuid(),
                Usuario = BuildUsuario("Antiguo"),
                SalaId = Guid.NewGuid(),
                Sala = BuildSala("S-1"),
                TiempoEstimado = 2,
                FechaSolicitud = DateTime.UtcNow.AddHours(-30),
                Estado = EstadoSolicitud.Pendiente
            },
            new()
            {
                Id = Guid.NewGuid(),
                UsuarioId = Guid.NewGuid(),
                Usuario = BuildUsuario("Reciente"),
                SalaId = Guid.NewGuid(),
                Sala = BuildSala("S-2"),
                TiempoEstimado = 1,
                FechaSolicitud = DateTime.UtcNow.AddHours(-10),
                Estado = EstadoSolicitud.Pendiente
            }
        };

        _solicitudRepository.Setup(r => r.GetSolicitudesByEstado(EstadoSolicitud.Pendiente))
            .ReturnsAsync(solicitudes);

        var resultado = await _service.GetSolicitudesUrgentesCount();

        Assert.Equal(1, resultado);
    }

    [Fact]
    public async Task AprobarSolicitud_SinEquipoSeleccionado_UsaDisponibleDeLaSala()
    {
        var salaId = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();
        var aprobadorId = Guid.NewGuid();
        var solicitud = new SolicitudPrestamo
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            Usuario = BuildUsuario("Solicitante"),
            SalaId = salaId,
            Sala = BuildSala("LAB-1"),
            Estado = EstadoSolicitud.Pendiente,
            TiempoEstimado = 2
        };

        var equipoDisponible = new Equipo
        {
            Id = Guid.NewGuid(),
            SalaId = salaId,
            Sala = solicitud.Sala,
            Estado = EstadoEquipo.Disponible
        };

        _solicitudRepository.Setup(r => r.GetSolicitud(solicitud.Id))
            .ReturnsAsync(solicitud);
        _equipoRepository.Setup(r => r.GetEquiposBySalaId(salaId))
            .ReturnsAsync(new List<Equipo> { equipoDisponible });
        _equipoRepository.Setup(r => r.GetEquipo(equipoDisponible.Id))
            .ReturnsAsync(equipoDisponible);

        await _service.AprobarSolicitud(solicitud.Id, aprobadorId);

        Assert.Equal(EstadoSolicitud.Aprobada, solicitud.Estado);
        Assert.Equal(equipoDisponible.Id, solicitud.EquipoId);
        Assert.Equal(aprobadorId, solicitud.AprobadoPorId);
        Assert.Equal(EstadoEquipo.Asignado, equipoDisponible.Estado);
        Assert.Equal(usuarioId, equipoDisponible.AsignadoAId);
        _solicitudRepository.Verify(r => r.Update(solicitud), Times.Once);
        _equipoRepository.Verify(r => r.Update(equipoDisponible), Times.Once);
    }

    [Fact]
    public async Task RechazarSolicitud_SinMotivo_LanzaArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.RechazarSolicitud(Guid.NewGuid(), Guid.NewGuid(), ""));
    }

    [Fact]
    public async Task CancelarSolicitud_CuandoUsuarioNoCoincide_LanzaInvalidOperation()
    {
        var solicitud = new SolicitudPrestamo
        {
            Id = Guid.NewGuid(),
            UsuarioId = Guid.NewGuid(),
            Usuario = BuildUsuario("Usuario"),
            SalaId = Guid.NewGuid(),
            Sala = BuildSala("Sala"),
            Estado = EstadoSolicitud.Pendiente,
            TiempoEstimado = 2
        };

        _solicitudRepository.Setup(r => r.GetSolicitud(solicitud.Id))
            .ReturnsAsync(solicitud);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.CancelarSolicitud(solicitud.Id, Guid.NewGuid()));
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
        Capacidad = 20,
        Ubicacion = "Edificio Central"
    };
}

