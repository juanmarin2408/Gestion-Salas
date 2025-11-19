using AutoMapper;
using Domain;
using Domain.Enums;
using Infrastructure.Repositories;
using Services.Models.SolicitudModels;
using Services.Models.UserModels;
using Services.Models.CalendarioModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class SolicitudPrestamoService : ISolicitudPrestamoService
    {
        private readonly ISolicitudPrestamoRepository _solicitudRepository;
        private readonly IEquipoRepository _equipoRepository;
        private readonly IMapper _mapper;

        public SolicitudPrestamoService(
            ISolicitudPrestamoRepository solicitudRepository, 
            IEquipoRepository equipoRepository,
            IMapper mapper)
        {
            _solicitudRepository = solicitudRepository;
            _equipoRepository = equipoRepository;
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

        public async Task<IList<OcupacionModel>> GetOcupacionesPorSemana(DateTime fechaSemana, Guid? salaId = null)
        {
            // Obtener el inicio de la semana (lunes) y el final (viernes)
            var inicioSemana = fechaSemana.Date;
            var diaSemana = (int)inicioSemana.DayOfWeek;
            if (diaSemana == 0) diaSemana = 7; // Domingo = 7
            var lunes = inicioSemana.AddDays(-(diaSemana - 1)); // Lunes
            var viernes = lunes.AddDays(4); // Viernes
            var inicioSemanaUtc = new DateTime(lunes.Year, lunes.Month, lunes.Day, 0, 0, 0, DateTimeKind.Utc);
            var finSemanaUtc = new DateTime(viernes.Year, viernes.Month, viernes.Day, 23, 59, 59, DateTimeKind.Utc);

            // Obtener todas las solicitudes aprobadas
            var todasSolicitudes = await _solicitudRepository.GetSolicitudes();
            var solicitudesAprobadas = todasSolicitudes
                .Where(s => s.Estado == EstadoSolicitud.Aprobada)
                .Where(s => !salaId.HasValue || s.SalaId == salaId.Value)
                .ToList();

            var ocupaciones = new List<OcupacionModel>();

            foreach (var solicitud in solicitudesAprobadas)
            {
                // Usar FechaAprobacion como fecha de inicio, o FechaSolicitud si no hay aprobación
                var fechaInicio = solicitud.FechaAprobacion ?? solicitud.FechaSolicitud;
                
                // Asegurar que sea UTC
                if (fechaInicio.Kind != DateTimeKind.Utc)
                {
                    fechaInicio = DateTime.SpecifyKind(fechaInicio, DateTimeKind.Utc);
                }

                // Calcular fecha fin basada en TiempoEstimado (horas)
                var fechaFin = fechaInicio.AddHours(solicitud.TiempoEstimado);

                // Verificar si la ocupación intersecta con la semana
                if (fechaFin >= inicioSemanaUtc && fechaInicio <= finSemanaUtc)
                {
                    ocupaciones.Add(new OcupacionModel
                    {
                        SolicitudId = solicitud.Id,
                        SalaId = solicitud.SalaId,
                        SalaNumero = solicitud.Sala.Numero,
                        UsuarioId = solicitud.UsuarioId,
                        UsuarioNombre = $"{solicitud.Usuario.Nombre} {solicitud.Usuario.Apellido}".Trim(),
                        FechaInicio = fechaInicio,
                        FechaFin = fechaFin,
                        TiempoEstimado = solicitud.TiempoEstimado,
                        EsSalaCompleta = !solicitud.EquipoId.HasValue // Si no hay equipo asignado, es sala completa
                    });
                }
            }

            return ocupaciones.OrderBy(o => o.FechaInicio).ToList();
        }

        public async Task Create(AddSolicitudModel model)
        {
            // Verificar si el usuario ya tiene un equipo activo asignado
            var equipos = await _equipoRepository.GetEquipos();
            var equipoActivo = equipos.FirstOrDefault(e => 
                e.AsignadoAId == model.UsuarioId && 
                e.Estado == EstadoEquipo.Asignado);

            if (equipoActivo != null)
            {
                throw new InvalidOperationException("Ya tienes un equipo activo asignado. No puedes solicitar otro equipo hasta que liberes el actual.");
            }

            var solicitud = new SolicitudPrestamo
            {
                Id = Guid.NewGuid(),
                UsuarioId = model.UsuarioId,
                SalaId = model.SalaId,
                EquipoId = model.EquipoId,
                TiempoEstimado = model.TiempoEstimado,
                FechaSolicitud = DateTime.UtcNow,
                Estado = EstadoSolicitud.Pendiente
            };

            await _solicitudRepository.Save(solicitud);
        }

        public async Task AprobarSolicitud(Guid solicitudId, Guid aprobadoPorId, Guid? equipoId = null)
        {
            var solicitud = await _solicitudRepository.GetSolicitud(solicitudId);
            if (solicitud == null)
            {
                throw new InvalidOperationException("Solicitud no encontrada.");
            }

            if (solicitud.Estado != EstadoSolicitud.Pendiente)
            {
                throw new InvalidOperationException("Solo se pueden aprobar solicitudes pendientes.");
            }

            // Si no se proporciona un equipo, buscar uno disponible en la sala
            if (!equipoId.HasValue)
            {
                var equiposDisponibles = await _equipoRepository.GetEquiposBySalaId(solicitud.SalaId);
                var equipoDisponible = equiposDisponibles.FirstOrDefault(e => e.Estado == EstadoEquipo.Disponible);
                
                if (equipoDisponible == null)
                {
                    throw new InvalidOperationException("No hay equipos disponibles en la sala solicitada.");
                }

                equipoId = equipoDisponible.Id;
            }

            // Obtener el equipo para actualizarlo (sin AsNoTracking)
            var equipo = await _equipoRepository.GetEquipo(equipoId.Value);
            if (equipo == null)
            {
                throw new InvalidOperationException("Equipo no encontrado.");
            }

            if (equipo.Estado != EstadoEquipo.Disponible)
            {
                throw new InvalidOperationException("El equipo seleccionado no está disponible.");
            }

            if (equipo.SalaId != solicitud.SalaId)
            {
                throw new InvalidOperationException("El equipo no pertenece a la sala solicitada.");
            }

            // Verificar si el usuario ya tiene un equipo activo asignado
            var equipos = await _equipoRepository.GetEquipos();
            var equipoActivo = equipos.FirstOrDefault(e => 
                e.AsignadoAId == solicitud.UsuarioId && 
                e.Estado == EstadoEquipo.Asignado &&
                e.Id != equipoId.Value); // Excluir el equipo que se está intentando asignar

            if (equipoActivo != null)
            {
                throw new InvalidOperationException("El usuario ya tiene un equipo activo asignado. No se puede aprobar la solicitud hasta que libere el equipo actual.");
            }

            // Asignar el equipo al usuario
            equipo.Estado = EstadoEquipo.Asignado;
            equipo.AsignadoAId = solicitud.UsuarioId;
            await _equipoRepository.Update(equipo);

            // Actualizar la solicitud
            solicitud.Estado = EstadoSolicitud.Aprobada;
            solicitud.EquipoId = equipoId.Value;
            solicitud.FechaAprobacion = DateTime.UtcNow;
            solicitud.AprobadoPorId = aprobadoPorId;
            solicitud.FechaRechazo = null;
            solicitud.MotivoRechazo = null;

            await _solicitudRepository.Update(solicitud);
        }

        public async Task RechazarSolicitud(Guid solicitudId, Guid rechazadoPorId, string motivoRechazo)
        {
            if (string.IsNullOrWhiteSpace(motivoRechazo))
            {
                throw new ArgumentException("El motivo del rechazo es obligatorio.", nameof(motivoRechazo));
            }

            var solicitud = await _solicitudRepository.GetSolicitud(solicitudId);
            if (solicitud == null)
            {
                throw new InvalidOperationException("Solicitud no encontrada.");
            }

            if (solicitud.Estado != EstadoSolicitud.Pendiente)
            {
                throw new InvalidOperationException("Solo se pueden rechazar solicitudes pendientes.");
            }

            // Actualizar la solicitud
            solicitud.Estado = EstadoSolicitud.Rechazada;
            solicitud.FechaRechazo = DateTime.UtcNow;
            solicitud.MotivoRechazo = motivoRechazo;
            solicitud.AprobadoPorId = rechazadoPorId; // Guardamos quien rechazó
            solicitud.FechaAprobacion = null;
            solicitud.EquipoId = null;

            await _solicitudRepository.Update(solicitud);
        }

        public async Task CancelarSolicitud(Guid solicitudId, Guid usuarioId)
        {
            var solicitud = await _solicitudRepository.GetSolicitud(solicitudId);
            if (solicitud == null)
            {
                throw new InvalidOperationException("Solicitud no encontrada.");
            }

            if (solicitud.UsuarioId != usuarioId)
            {
                throw new InvalidOperationException("No tienes permiso para cancelar esta solicitud.");
            }

            if (solicitud.Estado != EstadoSolicitud.Pendiente)
            {
                throw new InvalidOperationException("Solo se pueden cancelar solicitudes pendientes.");
            }

            // Marcar como rechazada con motivo de cancelación
            solicitud.Estado = EstadoSolicitud.Rechazada;
            solicitud.FechaRechazo = DateTime.UtcNow;
            solicitud.MotivoRechazo = "Cancelada por el usuario";
            solicitud.AprobadoPorId = null;
            solicitud.FechaAprobacion = null;
            solicitud.EquipoId = null;

            await _solicitudRepository.Update(solicitud);
        }
    }
}

