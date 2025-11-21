using Domain.Enums;
using System;

namespace Services.Models.AsesoriaModels
{
    public class AsesoriaModel
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioEmail { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public EstadoAsesoria Estado { get; set; }
        public PrioridadReporte Prioridad { get; set; }
        public string? MotivoRechazo { get; set; }
        public string? Observaciones { get; set; }
        public Guid? AtendidoPorId { get; set; }
        public string? AtendidoPorNombre { get; set; }
        public int NumeroTicket { get; set; }
    }
}




