using System;
using System.ComponentModel.DataAnnotations;

namespace Services.Models.ReporteModels
{
    public class AddReporteModel
    {
        [Required]
        public Guid UsuarioId { get; set; }

        [Required]
        public Domain.Enums.TipoReporte Tipo { get; set; }

        // EquipoId es requerido solo si Tipo == Equipo
        public Guid? EquipoId { get; set; }

        // SalaId es requerido solo si Tipo == Sala
        public Guid? SalaId { get; set; }

        [Required]
        [MaxLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        public Domain.Enums.PrioridadReporte Prioridad { get; set; } = Domain.Enums.PrioridadReporte.Media;
    }
}

