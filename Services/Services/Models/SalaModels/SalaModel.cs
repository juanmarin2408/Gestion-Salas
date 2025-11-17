using Domain.Enums;
using System;

namespace Services.Models.SalaModels
{
    public class SalaModel
    {
        public Guid Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public string Ubicacion { get; set; } = string.Empty;
        public EstadoSala Estado { get; set; }
        public Guid? UsuarioId { get; set; }
        public string? UsuarioNombre { get; set; }
        public int TotalEquipos { get; set; }
    }
}

