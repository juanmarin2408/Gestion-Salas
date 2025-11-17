using System.ComponentModel.DataAnnotations;

namespace Services.Models.UserModels
{
    public class ChangePasswordModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string NewPassword { get; set; } = string.Empty;

        [Required, Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

