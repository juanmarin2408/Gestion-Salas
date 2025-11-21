using Services.Models.AsesoriaModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IAsesoriaService
    {
        Task<IList<AsesoriaModel>> GetAsesorias();
        Task<AsesoriaModel?> GetAsesoria(Guid id);
        Task<IList<AsesoriaModel>> GetAsesoriasByEstado(Domain.Enums.EstadoAsesoria estado);
        Task<IList<AsesoriaModel>> GetAsesoriasByUsuarioId(Guid usuarioId);
        Task<int> GetAsesoriasPendientesCount();
        Task<int> GetAsesoriasEnProcesoCount();
        Task<int> GetAsesoriasResueltasCount();
        Task Create(AddAsesoriaModel model);
        Task AceptarAsesoria(Guid asesoriaId, Guid atendidoPorId);
        Task RechazarAsesoria(Guid asesoriaId, Guid rechazadoPorId, string motivoRechazo);
        Task ResolverAsesoria(Guid asesoriaId, Guid resueltoPorId, string observaciones);
    }
}




