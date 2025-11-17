using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class SolicitudPrestamo
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; } = null!;

        [Required]
        public Guid SalaId { get; set; }

        [ForeignKey("SalaId")]
        public Sala Sala { get; set; } = null!;

        // Equipo asignado cuando se aprueba la solicitud
        public Guid? EquipoId { get; set; }

        [ForeignKey("EquipoId")]
        public Equipo? Equipo { get; set; }

        // Tiempo estimado en horas
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El tiempo estimado debe ser mayor a 0")]
        public int TiempoEstimado { get; set; }

        [Required]
        public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;

        public DateTime? FechaAprobacion { get; set; }

        public DateTime? FechaRechazo { get; set; }

        public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;

        [MaxLength(500)]
        public string? MotivoRechazo { get; set; }

        [MaxLength(1000)]
        public string? Observaciones { get; set; }

        // Usuario que aprueba/rechaza (Coordinador o Admin)
        public Guid? AprobadoPorId { get; set; }

        [ForeignKey("AprobadoPorId")]
        public Usuario? AprobadoPor { get; set; }
    }
}

