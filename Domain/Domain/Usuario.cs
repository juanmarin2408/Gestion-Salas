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
        public string Nombre { get; set; }

        [MaxLength(100)]
        public string Apellido { get; set; }

        [Required, MaxLength(50)]
        public string Documento { get; set; }

        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

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

    }
}
