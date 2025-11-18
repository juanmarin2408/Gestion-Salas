using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Asesoria
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; } = null!;

        [Required]
        [MaxLength(1000)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaResolucion { get; set; }

        public EstadoAsesoria Estado { get; set; } = EstadoAsesoria.Pendiente;

        public PrioridadReporte Prioridad { get; set; } = PrioridadReporte.Media;

        [MaxLength(500)]
        public string? MotivoRechazo { get; set; }

        [MaxLength(1000)]
        public string? Observaciones { get; set; }

        // Usuario que atiende la asesoría (Coordinador)
        public Guid? AtendidoPorId { get; set; }

        [ForeignKey("AtendidoPorId")]
        public Usuario? AtendidoPor { get; set; }
    }
}

