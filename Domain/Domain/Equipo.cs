using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Equipo
    {
        [Key]
        public Guid Id { get; set; }     

        // Relaciónes

        public Guid SalaId { get; set; }
        public Sala Sala { get; set; } = null!;

        public EstadoEquipo Estado { get; set; } = EstadoEquipo.Disponible;

    }
}
