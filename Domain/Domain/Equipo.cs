using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Equipo
    {
        [Key]
        public Guid Id { get; set; }     

        [Required]
        public Guid SalaId { get; set; }

        [ForeignKey("SalaId")]
        public Sala Sala { get; set; } = null!;

        public EstadoEquipo Estado { get; set; } = EstadoEquipo.Disponible;

        // Usuario al que está asignado el equipo
        public Guid? AsignadoAId { get; set; }

        [ForeignKey("AsignadoAId")]
        public Usuario? AsignadoA { get; set; }

        // Motivo del bloqueo (cuando está en mantenimiento o dañado)
        [MaxLength(500)]
        public string? MotivoBloqueo { get; set; }

        // Prioridad del bloqueo
        public PrioridadReporte? PrioridadBloqueo { get; set; }

        // Fecha en que se bloqueó
        public DateTime? FechaBloqueo { get; set; }

    }
}
