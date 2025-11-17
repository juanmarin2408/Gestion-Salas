using Services.Models.SolicitudModels;
using Services.Models.UserModels;
using Services.Models.CalendarioModels;
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
        Task<IList<OcupacionModel>> GetOcupacionesPorSemana(DateTime fechaSemana, Guid? salaId = null);
        Task Create(AddSolicitudModel model);
        Task AprobarSolicitud(Guid solicitudId, Guid aprobadoPorId, Guid? equipoId = null);
        Task RechazarSolicitud(Guid solicitudId, Guid rechazadoPorId, string motivoRechazo);
        Task CancelarSolicitud(Guid solicitudId, Guid usuarioId);
    }
}

