using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IReporteDanoService
    {
        Task<IList<Services.Models.ReporteModels.ReporteModel>> GetReportes();
        Task<Services.Models.ReporteModels.ReporteModel?> GetReporte(Guid id);
        Task<IList<Services.Models.ReporteModels.ReporteModel>> GetReportesByEstado(Domain.Enums.EstadoReporte estado);
        Task<int> GetReportesPendientesCount();
        Task ResolverReporte(Guid reporteId, Guid resueltoPorId, string observaciones);
    }
}

