using AutoMapper;
using Domain;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Moq;
using Services;
using Services.Automapper;
using Services.Models.UserModels;

namespace ServicesTest;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher<Usuario>> _passwordHasher = new();
    private readonly IMapper _mapper;
    private readonly UserService _service;

    public UserServiceTests()
    {
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
        _mapper = mapperConfig.CreateMapper();
        _service = new UserService(_userRepository.Object, _mapper, _passwordHasher.Object);
    }

    [Fact]
    public async Task Register_EmailExistente_LanzaInvalidOperation()
    {
        var model = BuildAddUserModel();

        _userRepository.Setup(r => r.GetByEmail(model.Email))
            .ReturnsAsync(new Usuario());

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Register(model));
    }

    [Fact]
    public async Task Register_DocumentoExistente_LanzaInvalidOperation()
    {
        var model = BuildAddUserModel();

        _userRepository.Setup(r => r.GetByEmail(model.Email))
            .ReturnsAsync((Usuario?)null);
        _userRepository.Setup(r => r.GetByDocumento(model.Documento))
            .ReturnsAsync(new Usuario());

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Register(model));
    }

    [Fact]
    public async Task Register_GuardaUsuarioConPasswordHasheado()
    {
        var model = BuildAddUserModel();
        var hashedPassword = "hashed-secret";

        _userRepository.Setup(r => r.GetByEmail(model.Email))
            .ReturnsAsync((Usuario?)null);
        _userRepository.Setup(r => r.GetByDocumento(model.Documento))
            .ReturnsAsync((Usuario?)null);

        _passwordHasher.Setup(h => h.HashPassword(It.IsAny<Usuario>(), model.Password))
            .Returns(hashedPassword);

        Usuario? guardado = null;
        _userRepository.Setup(r => r.Save(It.IsAny<Usuario>()))
            .Callback<Usuario>(u => guardado = u)
            .Returns(Task.CompletedTask);

        await _service.Register(model);

        Assert.NotNull(guardado);
        Assert.Equal(hashedPassword, guardado!.PasswordHash);
        Assert.NotEqual(Guid.Empty, guardado.Id);
        _userRepository.Verify(r => r.Save(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_PasswordIncorrecto_RetornaNull()
    {
        var usuario = BuildUsuario();
        usuario.PasswordHash = "stored";

        _userRepository.Setup(r => r.GetByEmail(usuario.Email))
            .ReturnsAsync(usuario);
        _passwordHasher.Setup(h => h.VerifyHashedPassword(usuario, usuario.PasswordHash, "wrong"))
            .Returns(PasswordVerificationResult.Failed);

        var result = await _service.LoginAsync(usuario.Email, "wrong");

        Assert.Null(result);
        _userRepository.Verify(r => r.Update(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_Exitoso_ActualizaUltimoAcceso()
    {
        var usuario = BuildUsuario();
        usuario.PasswordHash = "stored";

        _userRepository.Setup(r => r.GetByEmail(usuario.Email))
            .ReturnsAsync(usuario);
        _passwordHasher.Setup(h => h.VerifyHashedPassword(usuario, usuario.PasswordHash, "secret"))
            .Returns(PasswordVerificationResult.Success);

        var result = await _service.LoginAsync(usuario.Email, "secret");

        Assert.NotNull(result);
        _userRepository.Verify(r => r.Update(usuario), Times.Once);
        Assert.True(usuario.UltimoAcceso.HasValue);
    }

    private static AddUserModel BuildAddUserModel() => new()
    {
        Nombre = "Ana",
        Apellido = "Ruiz",
        Documento = "DOC123",
        Email = "ana@example.com",
        Password = "Secreta123!",
        Rol = Domain.Enums.RolUsuario.Usuario
    };

    private static Usuario BuildUsuario() => new()
    {
        Id = Guid.NewGuid(),
        Nombre = "Carlos",
        Apellido = "Gómez",
        Documento = "DOC999",
        Email = "carlos@example.com",
        PasswordHash = "hash",
        Activo = true
    };
}

