using Domain;
using Domain.Enums;

namespace DomainTest;

public class DomainDefaultsTests
{
    [Fact]
    public void Usuario_InicializaColeccionesYValoresPorDefecto()
    {
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = "Ana",
            Apellido = "Pérez",
            Documento = "DOC123",
            Email = "ana@example.com",
            PasswordHash = "hash"
        };

        Assert.True(usuario.Activo);
        Assert.Equal(RolUsuario.Usuario, usuario.Rol);
        Assert.Equal(DateTimeKind.Utc, usuario.FechaCreacion.Kind);
        Assert.NotNull(usuario.Salas);
        Assert.Empty(usuario.Salas);
        Assert.NotNull(usuario.SolicitudesPrestamo);
        Assert.Empty(usuario.SolicitudesPrestamo);
        Assert.NotNull(usuario.ReportesDanos);
        Assert.Empty(usuario.ReportesDanos);
    }

    [Fact]
    public void Equipo_IniciaDisponibleSinAsignacionNiBloqueo()
    {
        var equipo = new Equipo
        {
            Id = Guid.NewGuid(),
            SalaId = Guid.NewGuid(),
            Sala = new Sala
            {
                Id = Guid.NewGuid(),
                Numero = "A-101",
                Capacidad = 10,
                Ubicacion = "Primer piso"
            }
        };

        Assert.Equal(EstadoEquipo.Disponible, equipo.Estado);
        Assert.Null(equipo.AsignadoAId);
        Assert.Null(equipo.MotivoBloqueo);
        Assert.Null(equipo.PrioridadBloqueo);
        Assert.Null(equipo.FechaBloqueo);
    }

    [Fact]
    public void SolicitudPrestamo_IniciaPendienteYConFechaUtc()
    {
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = "Luis",
            Apellido = "García",
            Documento = "DOC999",
            Email = "luis@example.com",
            PasswordHash = "hash"
        };

        var sala = new Sala
        {
            Id = Guid.NewGuid(),
            Numero = "B-202",
            Capacidad = 20,
            Ubicacion = "Segundo piso"
        };

        var solicitud = new SolicitudPrestamo
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            Usuario = usuario,
            SalaId = sala.Id,
            Sala = sala,
            TiempoEstimado = 3
        };

        Assert.Equal(EstadoSolicitud.Pendiente, solicitud.Estado);
        Assert.Equal(DateTimeKind.Utc, solicitud.FechaSolicitud.Kind);
        Assert.Null(solicitud.EquipoId);
        Assert.Null(solicitud.MotivoRechazo);
    }
}

