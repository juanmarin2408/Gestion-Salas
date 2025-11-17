using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Services.Models.SalaModels
{
    public class AddSalaModel
    {
        [Required, MaxLength(50)]
        public string Numero { get; set; } = string.Empty;

        [Required, Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser mayor a 0")]
        public int Capacidad { get; set; }

        [Required, MaxLength(200)]
        public string Ubicacion { get; set; } = string.Empty;

        public EstadoSala Estado { get; set; } = EstadoSala.Activa;

        public Guid? UsuarioId { get; set; }
    }
}

