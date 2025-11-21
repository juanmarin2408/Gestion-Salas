using Domain.Enums;
using System;

namespace Services.Models.SolicitudModels
{
    public class SolicitudModel
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioEmail { get; set; } = string.Empty;
        public Guid SalaId { get; set; }
        public string SalaNumero { get; set; } = string.Empty;
        public Guid? EquipoId { get; set; }
        public string? EquipoCodigo { get; set; }
        public int TiempoEstimado { get; set; }
        public TipoSolicitudPrestamo Tipo { get; set; } = TipoSolicitudPrestamo.Equipo;
        public DateTime? FechaInicioUso { get; set; }
        public DateTime? FechaFinUso { get; set; }
        public string? TituloUso { get; set; }
        public string? JustificacionUso { get; set; }
        public int? NumeroAsistentes { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public DateTime? FechaRechazo { get; set; }
        public EstadoSolicitud Estado { get; set; }
        public string? MotivoRechazo { get; set; }
        public string? Observaciones { get; set; }
        public Guid? AprobadoPorId { get; set; }
        public string? AprobadoPorNombre { get; set; }
    }
}

