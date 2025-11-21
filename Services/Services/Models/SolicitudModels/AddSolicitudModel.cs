using Domain.Enums;
using System;

namespace Services.Models.SolicitudModels
{
    public class AddSolicitudModel
    {
        public Guid UsuarioId { get; set; }
        public Guid SalaId { get; set; }
        public Guid? EquipoId { get; set; }
        public int TiempoEstimado { get; set; }
        public TipoSolicitudPrestamo Tipo { get; set; } = TipoSolicitudPrestamo.Equipo;
        public DateTime? FechaInicioUso { get; set; }
        public DateTime? FechaFinUso { get; set; }
        public string? TituloUso { get; set; }
        public string? JustificacionUso { get; set; }
        public int? NumeroAsistentes { get; set; }
    }
}

