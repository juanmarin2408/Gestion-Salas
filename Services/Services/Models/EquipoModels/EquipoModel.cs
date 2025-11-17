using Domain.Enums;
using System;

namespace Services.Models.EquipoModels
{
    public class EquipoModel
    {
        public Guid Id { get; set; }
        public Guid SalaId { get; set; }
        public string SalaNumero { get; set; } = string.Empty;
        public EstadoEquipo Estado { get; set; }
        public Guid? AsignadoAId { get; set; }
        public string? AsignadoANombre { get; set; }
        public string? AsignadoAEmail { get; set; }
        public string? MotivoBloqueo { get; set; }
        public PrioridadReporte? PrioridadBloqueo { get; set; }
        public DateTime? FechaBloqueo { get; set; }
    }
}

