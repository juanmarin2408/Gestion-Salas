using System;

namespace Services.Models.CalendarioModels
{
    public class OcupacionModel
    {
        public Guid SolicitudId { get; set; }
        public Guid SalaId { get; set; }
        public string SalaNumero { get; set; } = string.Empty;
        public Guid UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int TiempoEstimado { get; set; }
        public bool EsSalaCompleta { get; set; }
    }
}

