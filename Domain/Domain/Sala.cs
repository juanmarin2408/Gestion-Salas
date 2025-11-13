using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Sala
    {
        [Key]
        public Guid Id { get; set; }

        public string Numero { get; set; }

        public int Capacidad { get; set; }

        public string Ubicacion { get; set; }

        public EstadoSala Estado { get; set; }

        // Relación: una Sala tiene muchos Equipos
        public List<Equipo> Equipos { get; set; } = new();

        // Relación: una Sala puede tener un Usuario asignado (por ejemplo, responsable)
        public Guid? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

    }
}
