using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Usuario
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public required string Nombre { get; set; }

        [MaxLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public required string Documento { get; set; }

        [Required, EmailAddress, MaxLength(256)]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? UltimoAcceso { get; set; }

        public bool Activo { get; set; } = true;

        public RolUsuario Rol { get; set; } = RolUsuario.Usuario;

        // Relación: un Usuario puede ser responsable de varias Salas
        public List<Sala> Salas { get; set; } = new();

        // Relación: un Usuario puede tener muchas Solicitudes de Préstamo
        [InverseProperty("Usuario")]
        public List<SolicitudPrestamo> SolicitudesPrestamo { get; set; } = new();

        // Relación: un Usuario (Coordinador/Admin) puede aprobar muchas solicitudes
        [InverseProperty("AprobadoPor")]
        public List<SolicitudPrestamo> SolicitudesAprobadas { get; set; } = new();

        // Relación: un Usuario puede hacer muchos Reportes de Daños
        [InverseProperty("Usuario")]
        public List<ReporteDano> ReportesDanos { get; set; } = new();

        // Relación: un Usuario (Coordinador/Admin) puede resolver muchos reportes
        [InverseProperty("ResueltoPor")]
        public List<ReporteDano> ReportesResueltos { get; set; } = new();

        // Relación: un Usuario puede tener equipos asignados
        [InverseProperty("AsignadoA")]
        public List<Equipo> EquiposAsignados { get; set; } = new();

        // Relación: un Usuario puede tener muchas Solicitudes de Asesoría
        [InverseProperty("Usuario")]
        public List<Asesoria> Asesorias { get; set; } = new();

        // Relación: un Usuario (Coordinador) puede atender muchas asesorías
        [InverseProperty("AtendidoPor")]
        public List<Asesoria> AsesoriasAtendidas { get; set; } = new();

    }
}
