using Domain.Enums;
using System.ComponentModel.DataAnnotations;

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

    }
}
