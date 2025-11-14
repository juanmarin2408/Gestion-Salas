using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Services.Models.UserModels
{
    public class UserModel
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Apellido { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }

        public RolUsuario Rol { get; set; }
    }
}
