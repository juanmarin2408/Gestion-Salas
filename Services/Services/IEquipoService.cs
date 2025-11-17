using Services.Models.EquipoModels;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IEquipoService
    {
        Task<IList<EquipoModel>> GetEquipos();
        Task<EquipoModel?> GetEquipo(Guid id);
        Task<IList<EquipoModel>> GetEquiposBySalaId(Guid salaId);
        Task<IList<EquipoModel>> GetEquiposByEstado(EstadoEquipo estado);
        Task Create(AddEquipoModel model);
        Task Update(Guid id, AddEquipoModel model);
        Task Delete(Guid id);
        Task AsignarEquipo(Guid equipoId, Guid usuarioId);
        Task BloquearEquipo(Guid equipoId, string motivoBloqueo, PrioridadReporte prioridadBloqueo);
        Task LiberarEquipo(Guid equipoId);
    }
}

