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
    }
}

