using AutoMapper;
using Domain;
using Domain.Enums;
using Infrastructure.Repositories;
using Moq;
using Services;
using Services.Models.EquipoModels;

namespace ServicesTest;

public class EquipoServiceTests
{
    private readonly Mock<IEquipoRepository> _equipoRepository = new();
    private readonly Mock<ISalaRepository> _salaRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly EquipoService _service;

    public EquipoServiceTests()
    {
        _mapper.Setup(m => m.Map<Equipo>(It.IsAny<AddEquipoModel>()))
            .Returns((AddEquipoModel model) => new Equipo
            {
                SalaId = model.SalaId,
                Estado = model.Estado
            });

        _service = new EquipoService(_equipoRepository.Object, _salaRepository.Object, _mapper.Object);
    }

    [Fact]
    public async Task Create_CuandoSalaExiste_GuardaEquipoConNuevoId()
    {
        var salaId = Guid.NewGuid();
        var model = new AddEquipoModel { SalaId = salaId, Estado = EstadoEquipo.Disponible };

        _salaRepository.Setup(r => r.GetSala(salaId))
            .ReturnsAsync(new Sala
            {
                Id = salaId,
                Numero = "S1",
                Capacidad = 5,
                Ubicacion = "Primer piso"
            });

        Equipo? entidadGuardada = null;
        _equipoRepository.Setup(r => r.Save(It.IsAny<Equipo>()))
            .Callback<Equipo>(e => entidadGuardada = e)
            .Returns(Task.CompletedTask);

        await _service.Create(model);

        Assert.NotNull(entidadGuardada);
        Assert.Equal(salaId, entidadGuardada!.SalaId);
        Assert.NotEqual(Guid.Empty, entidadGuardada.Id);
        _equipoRepository.Verify(r => r.Save(It.IsAny<Equipo>()), Times.Once);
    }

    [Fact]
    public async Task Create_SalaInexistente_LanzaInvalidOperation()
    {
        var model = new AddEquipoModel { SalaId = Guid.NewGuid() };
        _salaRepository.Setup(r => r.GetSala(model.SalaId))
            .ReturnsAsync((Sala?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Create(model));
    }

    [Theory]
    [InlineData(PrioridadReporte.Alta, EstadoEquipo.Dañado)]
    [InlineData(PrioridadReporte.Urgente, EstadoEquipo.Dañado)]
    [InlineData(PrioridadReporte.Media, EstadoEquipo.EnMantenimiento)]
    public async Task BloquearEquipo_AjustaEstadoSegunPrioridad(PrioridadReporte prioridad, EstadoEquipo estadoEsperado)
    {
        var equipo = new Equipo
        {
            Id = Guid.NewGuid(),
            SalaId = Guid.NewGuid(),
            Sala = BuildSala(),
            Estado = EstadoEquipo.Disponible,
            AsignadoAId = Guid.NewGuid()
        };

        _equipoRepository.Setup(r => r.GetEquipo(equipo.Id))
            .ReturnsAsync(equipo);

        await _service.BloquearEquipo(equipo.Id, "Motivo", prioridad);

        Assert.Equal(estadoEsperado, equipo.Estado);
        Assert.Null(equipo.AsignadoAId);
        Assert.Equal("Motivo", equipo.MotivoBloqueo);
        Assert.Equal(prioridad, equipo.PrioridadBloqueo);
        Assert.NotNull(equipo.FechaBloqueo);
        _equipoRepository.Verify(r => r.Update(equipo), Times.Once);
    }

    [Fact]
    public async Task AsignarEquipo_NoDisponible_LanzaInvalidOperation()
    {
        var equipo = new Equipo
        {
            Id = Guid.NewGuid(),
            SalaId = Guid.NewGuid(),
            Sala = BuildSala(),
            Estado = EstadoEquipo.EnMantenimiento
        };

        _equipoRepository.Setup(r => r.GetEquipo(equipo.Id))
            .ReturnsAsync(equipo);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AsignarEquipo(equipo.Id, Guid.NewGuid()));
    }

    private static Sala BuildSala() => new()
    {
        Id = Guid.NewGuid(),
        Numero = "LAB-1",
        Capacidad = 30,
        Ubicacion = "Bloque A"
    };
}

