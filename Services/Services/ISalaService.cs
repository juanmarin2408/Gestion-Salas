using Services.Models.SalaModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface ISalaService
    {
        Task<IList<SalaModel>> GetSalas();
        Task<SalaModel?> GetSala(Guid id);
        Task<SalaModel?> GetSalaByNumero(string numero);
        Task<IList<SalaModel>> GetSalasByEstado(Domain.Enums.EstadoSala estado);
        Task<IList<SalaModel>> GetSalasByUsuarioId(Guid usuarioId);
        Task Create(AddSalaModel model);
        Task Update(Guid id, AddSalaModel model);
        Task Delete(Guid id);
    }
}

