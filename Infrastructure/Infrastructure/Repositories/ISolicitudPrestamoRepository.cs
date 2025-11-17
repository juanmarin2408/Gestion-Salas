using Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface ISolicitudPrestamoRepository
    {
        Task<IList<SolicitudPrestamo>> GetSolicitudes();
        Task<SolicitudPrestamo?> GetSolicitud(Guid id);
        Task<IList<SolicitudPrestamo>> GetSolicitudesByEstado(Domain.Enums.EstadoSolicitud estado);
        Task Save(SolicitudPrestamo solicitud);
        Task Update(SolicitudPrestamo solicitud);
        Task Delete(Guid id);
    }
}

