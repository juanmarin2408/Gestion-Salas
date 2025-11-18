using Domain.Enums;
using System;

namespace Services.Models.ReporteModels
{
    public class ReporteModel
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioEmail { get; set; } = string.Empty;
        public TipoReporte Tipo { get; set; }
        public Guid? EquipoId { get; set; }
        public string? EquipoCodigo { get; set; }
        public Guid? SalaId { get; set; }
        public string? SalaNumero { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaReporte { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public EstadoReporte Estado { get; set; }
        public PrioridadReporte Prioridad { get; set; }
        public string? Observaciones { get; set; }
        public Guid? ResueltoPorId { get; set; }
        public string? ResueltoPorNombre { get; set; }
    }
}

