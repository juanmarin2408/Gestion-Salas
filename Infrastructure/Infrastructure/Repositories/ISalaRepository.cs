using Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface ISalaRepository
    {
        Task<IList<Sala>> GetSalas();
        Task<Sala?> GetSala(Guid id);
        Task<Sala?> GetSalaByNumero(string numero);
        Task<IList<Sala>> GetSalasByEstado(Domain.Enums.EstadoSala estado);
        Task<IList<Sala>> GetSalasByUsuarioId(Guid usuarioId);
        Task Save(Sala sala);
        Task Update(Sala sala);
        Task Delete(Guid id);
    }
}

