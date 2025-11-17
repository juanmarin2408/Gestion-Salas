using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class ReporteDano
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; } = null!;

        [Required]
        public Guid EquipoId { get; set; }

        [ForeignKey("EquipoId")]
        public Equipo Equipo { get; set; } = null!;

        // La Sala se obtiene a través del Equipo para evitar ciclos de cascada

        [Required]
        [MaxLength(1000)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public DateTime FechaReporte { get; set; } = DateTime.UtcNow;

        public DateTime? FechaResolucion { get; set; }

        public EstadoReporte Estado { get; set; } = EstadoReporte.Pendiente;

        public PrioridadReporte Prioridad { get; set; } = PrioridadReporte.Media;

        [MaxLength(1000)]
        public string? Observaciones { get; set; }

        // Usuario que resuelve el reporte (Coordinador o Admin)
        public Guid? ResueltoPorId { get; set; }

        [ForeignKey("ResueltoPorId")]
        public Usuario? ResueltoPor { get; set; }
    }
}

