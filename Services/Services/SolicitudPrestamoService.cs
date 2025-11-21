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
using System.Text.Json;

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
            return solicitudes.Select(MapSolicitudModel).ToList();
        }

        public async Task<SolicitudModel?> GetSolicitud(Guid id)
        {
            var solicitud = await _solicitudRepository.GetSolicitud(id);
            if (solicitud == null) return null;

            return MapSolicitudModel(solicitud);
        }

        public async Task<IList<SolicitudModel>> GetSolicitudesByEstado(EstadoSolicitud estado)
        {
            var solicitudes = await _solicitudRepository.GetSolicitudesByEstado(estado);
            return solicitudes.Select(MapSolicitudModel).ToList();
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
            var inicioSemana = fechaSemana.Date;
            var diaSemana = (int)inicioSemana.DayOfWeek;
            if (diaSemana == 0) diaSemana = 7;
            var lunes = inicioSemana.AddDays(-(diaSemana - 1));
            var viernes = lunes.AddDays(4);
            var inicioSemanaUtc = new DateTime(lunes.Year, lunes.Month, lunes.Day, 0, 0, 0, DateTimeKind.Utc);
            var finSemanaUtc = new DateTime(viernes.Year, viernes.Month, viernes.Day, 23, 59, 59, DateTimeKind.Utc);

            var todasSolicitudes = await _solicitudRepository.GetSolicitudes();
            var solicitudesAprobadas = todasSolicitudes
                .Where(s => s.Estado == EstadoSolicitud.Aprobada)
                .Where(s => !salaId.HasValue || s.SalaId == salaId.Value)
                .ToList();

            var ocupaciones = new List<OcupacionModel>();

            foreach (var solicitud in solicitudesAprobadas)
            {
                var (fechaInicio, fechaFin) = ObtenerVentanaUso(solicitud);

                if (fechaFin >= inicioSemanaUtc && fechaInicio <= finSemanaUtc)
                {
                    var esSala = EsSolicitudSala(solicitud);
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
                        EsSalaCompleta = esSala || !solicitud.EquipoId.HasValue
                    });
                }
            }

            return ocupaciones.OrderBy(o => o.FechaInicio).ToList();
        }

        public async Task Create(AddSolicitudModel model)
        {
            if (model.Tipo == TipoSolicitudPrestamo.SalaCompleta)
            {
                await CrearSolicitudSalaCompleta(model);
                return;
            }

            await CrearSolicitudEquipo(model);
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

            if (EsSolicitudSala(solicitud))
            {
                await AprobarReservaSala(solicitud, aprobadoPorId);
            }
            else
            {
                await AprobarSolicitudEquipo(solicitud, aprobadoPorId, equipoId);
            }
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

        private static SolicitudModel MapSolicitudModel(SolicitudPrestamo solicitud)
        {
            var (metadata, notas) = ParseSalaMetadata(solicitud.Observaciones);

            var modelo = new SolicitudModel
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
                Observaciones = notas,
                AprobadoPorId = solicitud.AprobadoPorId,
                AprobadoPorNombre = solicitud.AprobadoPor != null ? $"{solicitud.AprobadoPor.Nombre} {solicitud.AprobadoPor.Apellido}".Trim() : null
            };

            if (metadata != null)
            {
                modelo.Tipo = TipoSolicitudPrestamo.SalaCompleta;
                modelo.FechaInicioUso = metadata.FechaInicioUtc.ToLocalTime();
                modelo.FechaFinUso = metadata.FechaFinUtc.ToLocalTime();
                modelo.TituloUso = metadata.Titulo;
                modelo.JustificacionUso = metadata.Motivo;
                modelo.NumeroAsistentes = metadata.Asistentes;
            }
            else
            {
                modelo.Tipo = TipoSolicitudPrestamo.Equipo;
            }

            return modelo;
        }

        private async Task CrearSolicitudSalaCompleta(AddSolicitudModel model)
        {
            if (!model.FechaInicioUso.HasValue || !model.FechaFinUso.HasValue)
            {
                throw new InvalidOperationException("Debes indicar la fecha y hora de inicio y fin del préstamo de la sala.");
            }

            var fechaInicio = EnsureUtc(model.FechaInicioUso.Value);
            var fechaFin = EnsureUtc(model.FechaFinUso.Value);

            if (fechaFin <= fechaInicio)
            {
                throw new InvalidOperationException("La hora de finalización debe ser mayor a la hora de inicio.");
            }

            var solicitudes = await _solicitudRepository.GetSolicitudes();
            var existeTraslape = solicitudes
                .Where(s => s.SalaId == model.SalaId && s.Estado != EstadoSolicitud.Rechazada)
                .Any(s =>
                {
                    var (metadata, _) = ParseSalaMetadata(s.Observaciones);
                    if (metadata == null)
                    {
                        return false;
                    }

                    return IntervalsOverlap(metadata.FechaInicioUtc, metadata.FechaFinUtc, fechaInicio, fechaFin);
                });

            if (existeTraslape)
            {
                throw new InvalidOperationException("La sala ya tiene una solicitud registrada dentro del rango seleccionado.");
            }

            var metadata = new SalaMetadata
            {
                FechaInicioUtc = fechaInicio,
                FechaFinUtc = fechaFin,
                Titulo = model.TituloUso?.Trim(),
                Motivo = model.JustificacionUso?.Trim(),
                Asistentes = model.NumeroAsistentes
            };

            var solicitud = new SolicitudPrestamo
            {
                Id = Guid.NewGuid(),
                UsuarioId = model.UsuarioId,
                SalaId = model.SalaId,
                TiempoEstimado = Math.Max(1, (int)Math.Ceiling((fechaFin - fechaInicio).TotalHours)),
                FechaSolicitud = DateTime.UtcNow,
                Estado = EstadoSolicitud.Pendiente
            };

            GuardarSalaMetadata(solicitud, metadata);

            await _solicitudRepository.Save(solicitud);
        }

        private async Task CrearSolicitudEquipo(AddSolicitudModel model)
        {
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

        private async Task AprobarReservaSala(SolicitudPrestamo solicitud, Guid aprobadoPorId)
        {
            var (metadata, notas) = ParseSalaMetadata(solicitud.Observaciones);
            if (metadata == null)
            {
                throw new InvalidOperationException("La solicitud de sala no tiene datos de agenda.");
            }

            var solicitudes = await _solicitudRepository.GetSolicitudes();
            var conflicto = solicitudes
                .Where(s => s.Id != solicitud.Id && s.SalaId == solicitud.SalaId && s.Estado == EstadoSolicitud.Aprobada)
                .Any(s =>
                {
                    var (metaExistente, _) = ParseSalaMetadata(s.Observaciones);
                    if (metaExistente == null)
                    {
                        return false;
                    }

                    return IntervalsOverlap(metaExistente.FechaInicioUtc, metaExistente.FechaFinUtc, metadata.FechaInicioUtc, metadata.FechaFinUtc);
                });

            if (conflicto)
            {
                throw new InvalidOperationException("La sala ya fue aprobada para ese rango horario.");
            }

            // Aseguramos que la metadata siga almacenada junto con posibles notas
            GuardarSalaMetadata(solicitud, metadata, notas);

            solicitud.Estado = EstadoSolicitud.Aprobada;
            solicitud.FechaAprobacion = DateTime.UtcNow;
            solicitud.AprobadoPorId = aprobadoPorId;
            solicitud.FechaRechazo = null;
            solicitud.MotivoRechazo = null;

            await _solicitudRepository.Update(solicitud);
        }

        private async Task AprobarSolicitudEquipo(SolicitudPrestamo solicitud, Guid aprobadoPorId, Guid? equipoId)
        {
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

            var equipos = await _equipoRepository.GetEquipos();
            var equipoActivo = equipos.FirstOrDefault(e =>
                e.AsignadoAId == solicitud.UsuarioId &&
                e.Estado == EstadoEquipo.Asignado &&
                e.Id != equipoId.Value);

            if (equipoActivo != null)
            {
                throw new InvalidOperationException("El usuario ya tiene un equipo activo asignado. No se puede aprobar la solicitud hasta que libere el equipo actual.");
            }

            equipo.Estado = EstadoEquipo.Asignado;
            equipo.AsignadoAId = solicitud.UsuarioId;
            await _equipoRepository.Update(equipo);

            solicitud.Estado = EstadoSolicitud.Aprobada;
            solicitud.EquipoId = equipoId.Value;
            solicitud.FechaAprobacion = DateTime.UtcNow;
            solicitud.AprobadoPorId = aprobadoPorId;
            solicitud.FechaRechazo = null;
            solicitud.MotivoRechazo = null;

            await _solicitudRepository.Update(solicitud);
        }

        private static (DateTime inicio, DateTime fin) ObtenerVentanaUso(SolicitudPrestamo solicitud)
        {
            var (metadata, _) = ParseSalaMetadata(solicitud.Observaciones);
            if (metadata != null)
            {
                return (metadata.FechaInicioUtc, metadata.FechaFinUtc);
            }

            var inicio = EnsureUtc(solicitud.FechaAprobacion ?? solicitud.FechaSolicitud);
            var fin = inicio.AddHours(solicitud.TiempoEstimado <= 0 ? 1 : solicitud.TiempoEstimado);
            return (inicio, fin);
        }

        private static DateTime EnsureUtc(DateTime dateTime)
        {
            return dateTime.Kind switch
            {
                DateTimeKind.Utc => dateTime,
                DateTimeKind.Local => dateTime.ToUniversalTime(),
                _ => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
            };
        }

        private static bool IntervalsOverlap(DateTime startA, DateTime endA, DateTime startB, DateTime endB)
        {
            return startA < endB && startB < endA;
        }

        private const string SalaMetaPrefix = "__SALA_META__";

        private sealed record SalaMetadata
        {
            public DateTime FechaInicioUtc { get; init; }
            public DateTime FechaFinUtc { get; init; }
            public string? Titulo { get; init; }
            public string? Motivo { get; init; }
            public int? Asistentes { get; init; }
        }

        private static void GuardarSalaMetadata(SolicitudPrestamo solicitud, SalaMetadata metadata, string? notas = null)
        {
            var json = JsonSerializer.Serialize(metadata);
            solicitud.Observaciones = string.IsNullOrEmpty(notas)
                ? $"{SalaMetaPrefix}{json}"
                : $"{SalaMetaPrefix}{json}\n{notas}";
        }

        private static (SalaMetadata? metadata, string? notas) ParseSalaMetadata(string? observaciones)
        {
            if (string.IsNullOrWhiteSpace(observaciones) || !observaciones.StartsWith(SalaMetaPrefix, StringComparison.Ordinal))
            {
                return (null, observaciones);
            }

            var content = observaciones.Substring(SalaMetaPrefix.Length);
            string? notas = null;
            var newlineIndex = content.IndexOf('\n');
            if (newlineIndex >= 0)
            {
                notas = content[(newlineIndex + 1)..];
                content = content.Substring(0, newlineIndex);
            }

            try
            {
                var metadata = JsonSerializer.Deserialize<SalaMetadata>(content);
                return (metadata, notas);
            }
            catch
            {
                return (null, observaciones);
            }
        }

        private static bool EsSolicitudSala(SolicitudPrestamo solicitud)
            => ParseSalaMetadata(solicitud.Observaciones).metadata != null;
    }
}

