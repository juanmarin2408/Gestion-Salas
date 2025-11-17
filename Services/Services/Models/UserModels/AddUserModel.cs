using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Services.Models.UserModels
{
    public class AddUserModel
    {
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Documento { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required, Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public RolUsuario Rol { get; set; } = RolUsuario.Usuario;
    }
}
