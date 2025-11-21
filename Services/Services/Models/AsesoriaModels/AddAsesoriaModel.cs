using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Services.Models.AsesoriaModels
{
    public class AddAsesoriaModel
    {
        [Required]
        public Guid UsuarioId { get; set; }

        [Required]
        [MaxLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        public PrioridadReporte Prioridad { get; set; } = PrioridadReporte.Media;
    }
}




