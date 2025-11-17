using AutoMapper;
using Domain;
using Domain.Enums;
using Infrastructure.Repositories;
using Services.Models.SolicitudModels;
using Services.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class SolicitudPrestamoService : ISolicitudPrestamoService
    {
        private readonly ISolicitudPrestamoRepository _solicitudRepository;
        private readonly IMapper _mapper;

        public SolicitudPrestamoService(ISolicitudPrestamoRepository solicitudRepository, IMapper mapper)
        {
            _solicitudRepository = solicitudRepository;
            _mapper = mapper;
        }

        public async Task<IList<SolicitudModel>> GetSolicitudes()
        {
            var solicitudes = await _solicitudRepository.GetSolicitudes();
            return solicitudes.Select(s => new SolicitudModel
            {
                Id = s.Id,
                UsuarioId = s.UsuarioId,
                UsuarioNombre = $"{s.Usuario.Nombre} {s.Usuario.Apellido}".Trim(),
                UsuarioEmail = s.Usuario.Email,
                SalaId = s.SalaId,
                SalaNumero = s.Sala.Numero,
                EquipoId = s.EquipoId,
                EquipoCodigo = s.Equipo != null ? s.Equipo.Id.ToString().Substring(0, 8).ToUpper() : null,
                TiempoEstimado = s.TiempoEstimado,
                FechaSolicitud = s.FechaSolicitud,
                FechaAprobacion = s.FechaAprobacion,
                FechaRechazo = s.FechaRechazo,
                Estado = s.Estado,
                MotivoRechazo = s.MotivoRechazo,
                Observaciones = s.Observaciones,
                AprobadoPorId = s.AprobadoPorId,
                AprobadoPorNombre = s.AprobadoPor != null ? $"{s.AprobadoPor.Nombre} {s.AprobadoPor.Apellido}".Trim() : null
            }).ToList();
        }

        public async Task<SolicitudModel?> GetSolicitud(Guid id)
        {
            var solicitud = await _solicitudRepository.GetSolicitud(id);
            if (solicitud == null) return null;

            return new SolicitudModel
            {
                Id = solicitud.Id,
                UsuarioId = solicitud.UsuarioId,
                UsuarioNombre = $"{solicitud.Usuario.Nombre} {solicitud.Usuario.Apellido}".Trim(),
                UsuarioEmail = solicitud.Usuario.Email,
                SalaId = solicitud.SalaId,
                SalaNumero = solicitud.Sala.Numero,
                EquipoId = solicitud.EquipoId,
                EquipoCodigo = solicitud.Equipo != null ? solicitud.Equipo.Id.ToString().Substring(0, 8).ToUpper() : null,
                TiempoEstimado = solicitud.TiempoEstimado,
                FechaSolicitud = solicitud.FechaSolicitud,
                FechaAprobacion = solicitud.FechaAprobacion,
                FechaRechazo = solicitud.FechaRechazo,
                Estado = solicitud.Estado,
                MotivoRechazo = solicitud.MotivoRechazo,
                Observaciones = solicitud.Observaciones,
                AprobadoPorId = solicitud.AprobadoPorId,
                AprobadoPorNombre = solicitud.AprobadoPor != null ? $"{solicitud.AprobadoPor.Nombre} {solicitud.AprobadoPor.Apellido}".Trim() : null
            };
        }

        public async Task<IList<SolicitudModel>> GetSolicitudesByEstado(EstadoSolicitud estado)
        {
            var solicitudes = await _solicitudRepository.GetSolicitudesByEstado(estado);
            return solicitudes.Select(s => new SolicitudModel
            {
                Id = s.Id,
                UsuarioId = s.UsuarioId,
                UsuarioNombre = $"{s.Usuario.Nombre} {s.Usuario.Apellido}".Trim(),
                UsuarioEmail = s.Usuario.Email,
                SalaId = s.SalaId,
                SalaNumero = s.Sala.Numero,
                EquipoId = s.EquipoId,
                EquipoCodigo = s.Equipo != null ? s.Equipo.Id.ToString().Substring(0, 8).ToUpper() : null,
                TiempoEstimado = s.TiempoEstimado,
                FechaSolicitud = s.FechaSolicitud,
                FechaAprobacion = s.FechaAprobacion,
                FechaRechazo = s.FechaRechazo,
                Estado = s.Estado,
                MotivoRechazo = s.MotivoRechazo,
                Observaciones = s.Observaciones,
                AprobadoPorId = s.AprobadoPorId,
                AprobadoPorNombre = s.AprobadoPor != null ? $"{s.AprobadoPor.Nombre} {s.AprobadoPor.Apellido}".Trim() : null
            }).ToList();
        }

        public async Task<int> GetSolicitudesPendientesCount()
        {
            var solicitudes = await _solicitudRepository.GetSolicitudesByEstado(EstadoSolicitud.Pendiente);
            return solicitudes.Count;
        }

        public async Task<int> GetSolicitudesUrgentesCount()
        {
            var solicitudes = await _solicitudRepository.GetSolicitudesByEstado(EstadoSolicitud.Pendiente);
            var ahora = DateTime.UtcNow;
            return solicitudes.Count(s => s.FechaSolicitud.AddHours(24) < ahora);
        }

        public async Task<IList<UserModel>> GetUsuariosConSolicitudes()
        {
            // Obtener todas las solicitudes (pendientes y aprobadas)
            var todasSolicitudes = await _solicitudRepository.GetSolicitudes();
            var solicitudesValidas = todasSolicitudes
                .Where(s => s.Estado == EstadoSolicitud.Pendiente || s.Estado == EstadoSolicitud.Aprobada)
                .ToList();

            // Obtener usuarios únicos de las solicitudes
            var usuariosUnicos = solicitudesValidas
                .Select(s => s.Usuario)
                .Where(u => u != null)
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .Select(u => new UserModel
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    Documento = u.Documento,
                    Email = u.Email,
                    Rol = u.Rol,
                    Activo = u.Activo
                })
                .ToList();

            return usuariosUnicos;
        }
    }
}

