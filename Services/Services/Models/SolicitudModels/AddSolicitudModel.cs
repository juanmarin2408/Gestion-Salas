using System;

namespace Services.Models.SolicitudModels
{
    public class AddSolicitudModel
    {
        public Guid UsuarioId { get; set; }
        public Guid SalaId { get; set; }
        public Guid? EquipoId { get; set; }
        public int TiempoEstimado { get; set; }
    }
}

