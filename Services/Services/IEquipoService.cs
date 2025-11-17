using Services.Models.EquipoModels;
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
        Task<IList<EquipoModel>> GetEquiposByEstado(Domain.Enums.EstadoEquipo estado);
        Task Create(AddEquipoModel model);
        Task Update(Guid id, AddEquipoModel model);
        Task Delete(Guid id);
    }
}

