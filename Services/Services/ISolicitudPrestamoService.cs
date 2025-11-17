using Services.Models.SolicitudModels;
using Services.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface ISolicitudPrestamoService
    {
        Task<IList<SolicitudModel>> GetSolicitudes();
        Task<SolicitudModel?> GetSolicitud(Guid id);
        Task<IList<SolicitudModel>> GetSolicitudesByEstado(Domain.Enums.EstadoSolicitud estado);
        Task<int> GetSolicitudesPendientesCount();
        Task<int> GetSolicitudesUrgentesCount();
        Task<IList<UserModel>> GetUsuariosConSolicitudes();
    }
}

