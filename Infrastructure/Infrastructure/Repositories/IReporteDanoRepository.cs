using Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IReporteDanoRepository
    {
        Task<IList<ReporteDano>> GetReportes();
        Task<ReporteDano?> GetReporte(Guid id);
        Task<IList<ReporteDano>> GetReportesByEstado(Domain.Enums.EstadoReporte estado);
        Task Save(ReporteDano reporte);
        Task Update(ReporteDano reporte);
        Task Delete(Guid id);
    }
}

