using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Services.Models.EquipoModels
{
    public class AddEquipoModel
    {
        [Required(ErrorMessage = "La sala es requerida")]
        public Guid SalaId { get; set; }

        public EstadoEquipo Estado { get; set; } = EstadoEquipo.Disponible;
    }
}

