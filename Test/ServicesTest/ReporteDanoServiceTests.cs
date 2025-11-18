using AutoMapper;
using Domain;
using Domain.Enums;
using Infrastructure.Repositories;
using Moq;
using Services;
using Services.Models.ReporteModels;

namespace ServicesTest;

public class ReporteDanoServiceTests
{
    private readonly Mock<IReporteDanoRepository> _reporteRepository = new();
    private readonly Mock<IEquipoRepository> _equipoRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly ReporteDanoService _service;

    public ReporteDanoServiceTests()
    {
        _service = new ReporteDanoService(_reporteRepository.Object, _equipoRepository.Object, _mapper.Object);
    }

    [Fact]
    public async Task Create_ReporteDeEquipo_CopiaSalaDelEquipo()
    {
        var equipo = new Equipo
        {
            Id = Guid.NewGuid(),
            SalaId = Guid.NewGuid(),
            Sala = new Sala
            {
                Id = Guid.NewGuid(),
                Numero = "LAB-2",
                Capacidad = 15,
                Ubicacion = "Edificio B"
            }
        };

        _equipoRepository.Setup(r => r.GetEquipo(equipo.Id))
            .ReturnsAsync(equipo);

        ReporteDano? guardado = null;
        _reporteRepository.Setup(r => r.Save(It.IsAny<ReporteDano>()))
            .Callback<ReporteDano>(r => guardado = r)
            .Returns(Task.CompletedTask);

        var modelo = new AddReporteModel
        {
            UsuarioId = Guid.NewGuid(),
            Tipo = TipoReporte.Equipo,
            EquipoId = equipo.Id,
            Descripcion = "Pantalla dañada",
            Prioridad = PrioridadReporte.Alta
        };

        await _service.Create(modelo);

        Assert.NotNull(guardado);
        Assert.Equal(equipo.Id, guardado!.EquipoId);
        Assert.Equal(equipo.SalaId, guardado.SalaId);
        Assert.Equal(EstadoReporte.Pendiente, guardado.Estado);
    }

    [Fact]
    public async Task Create_ReporteEquipoSinEquipoId_LanzaInvalidOperation()
    {
        var modelo = new AddReporteModel
        {
            UsuarioId = Guid.NewGuid(),
            Tipo = TipoReporte.Equipo,
            Descripcion = "Falta ID"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Create(modelo));
    }

    [Fact]
    public async Task ResolverReporte_YaResuelto_LanzaInvalidOperation()
    {
        var reporte = new ReporteDano
        {
            Id = Guid.NewGuid(),
            UsuarioId = Guid.NewGuid(),
            Usuario = BuildUsuario(),
            Tipo = TipoReporte.Equipo,
            Descripcion = "Listo",
            Estado = EstadoReporte.Resuelto
        };

        _reporteRepository.Setup(r => r.GetReporte(reporte.Id))
            .ReturnsAsync(reporte);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.ResolverReporte(reporte.Id, Guid.NewGuid(), "ok"));
    }

    private static Usuario BuildUsuario() => new()
    {
        Id = Guid.NewGuid(),
        Nombre = "Carlos",
        Apellido = "Núñez",
        Documento = "DOC888",
        Email = "carlos@example.com",
        PasswordHash = "hash"
    };
}

