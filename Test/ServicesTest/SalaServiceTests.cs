using AutoMapper;
using Domain;
using Domain.Enums;
using Infrastructure.Repositories;
using Moq;
using Services;
using Services.Automapper;
using Services.Models.SalaModels;

namespace ServicesTest;

public class SalaServiceTests
{
    private readonly Mock<ISalaRepository> _salaRepository = new();
    private readonly IMapper _mapper;
    private readonly SalaService _service;

    public SalaServiceTests()
    {
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
        _mapper = mapperConfig.CreateMapper();
        _service = new SalaService(_salaRepository.Object, _mapper);
    }

    [Fact]
    public async Task Create_NumeroDuplicado_LanzaInvalidOperation()
    {
        var model = new AddSalaModel
        {
            Numero = "LAB-100",
            Capacidad = 20,
            Ubicacion = "Ala Norte",
            Estado = EstadoSala.Activa
        };

        _salaRepository.Setup(r => r.GetSalaByNumero(model.Numero))
            .ReturnsAsync(new Sala());

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Create(model));
    }

    [Fact]
    public async Task Create_GeneraIdNuevoYGuarda()
    {
        var model = new AddSalaModel
        {
            Numero = "LAB-200",
            Capacidad = 30,
            Ubicacion = "Bloque B",
            Estado = EstadoSala.Activa,
            UsuarioId = Guid.NewGuid()
        };

        _salaRepository.Setup(r => r.GetSalaByNumero(model.Numero))
            .ReturnsAsync((Sala?)null);

        Sala? entidadGuardada = null;
        _salaRepository.Setup(r => r.Save(It.IsAny<Sala>()))
            .Callback<Sala>(s => entidadGuardada = s)
            .Returns(Task.CompletedTask);

        await _service.Create(model);

        Assert.NotNull(entidadGuardada);
        Assert.Equal(model.Numero, entidadGuardada!.Numero);
        Assert.NotEqual(Guid.Empty, entidadGuardada.Id);
        _salaRepository.Verify(r => r.Save(It.IsAny<Sala>()), Times.Once);
    }

    [Fact]
    public async Task Update_SalaNoExiste_LanzaInvalidOperation()
    {
        var salaId = Guid.NewGuid();
        _salaRepository.Setup(r => r.GetSala(salaId))
            .ReturnsAsync((Sala?)null);

        var model = new AddSalaModel
        {
            Numero = "LAB-300",
            Capacidad = 15,
            Ubicacion = "Bloque C",
            Estado = EstadoSala.Inactiva
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Update(salaId, model));
    }

    [Fact]
    public async Task Update_NumeroUsadoPorOtraSala_LanzaInvalidOperation()
    {
        var salaId = Guid.NewGuid();
        var existente = new Sala
        {
            Id = salaId,
            Numero = "LAB-400",
            Capacidad = 10,
            Ubicacion = "Bloque D",
            Estado = EstadoSala.Activa
        };

        var otraSala = new Sala
        {
            Id = Guid.NewGuid(),
            Numero = "LAB-500",
            Capacidad = 12,
            Ubicacion = "Bloque E",
            Estado = EstadoSala.Activa
        };

        _salaRepository.Setup(r => r.GetSala(salaId))
            .ReturnsAsync(existente);
        _salaRepository.Setup(r => r.GetSalaByNumero(otraSala.Numero))
            .ReturnsAsync(otraSala);

        var model = new AddSalaModel
        {
            Numero = otraSala.Numero,
            Capacidad = 20,
            Ubicacion = "Remodelada",
            Estado = EstadoSala.Activa
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Update(salaId, model));
    }

    [Fact]
    public async Task Delete_SalaNoExiste_LanzaInvalidOperation()
    {
        var salaId = Guid.NewGuid();
        _salaRepository.Setup(r => r.GetSala(salaId))
            .ReturnsAsync((Sala?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Delete(salaId));
    }
}

