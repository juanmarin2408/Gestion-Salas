using Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IEquipoRepository
    {
        Task<IList<Equipo>> GetEquipos();
        Task<Equipo?> GetEquipo(Guid id);
        Task<IList<Equipo>> GetEquiposBySalaId(Guid salaId);
        Task<IList<Equipo>> GetEquiposByEstado(Domain.Enums.EstadoEquipo estado);
        Task Save(Equipo equipo);
        Task Update(Equipo equipo);
        Task Delete(Guid id);
    }
}

