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

    }
}
