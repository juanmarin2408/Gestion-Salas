using Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IAsesoriaRepository
    {
        Task<IList<Asesoria>> GetAsesorias();
        Task<Asesoria?> GetAsesoria(Guid id);
        Task<IList<Asesoria>> GetAsesoriasByEstado(Domain.Enums.EstadoAsesoria estado);
        Task<IList<Asesoria>> GetAsesoriasByUsuarioId(Guid usuarioId);
        Task Save(Asesoria asesoria);
        Task Update(Asesoria asesoria);
        Task Delete(Guid id);
    }
}



