using AutoMapper;
using Domain;
using Domain.Enums;
using Infrastructure.Repositories;
using Services.Models.AsesoriaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class AsesoriaService : IAsesoriaService
    {
        private readonly IAsesoriaRepository _asesoriaRepository;
        private readonly IMapper _mapper;

        public AsesoriaService(IAsesoriaRepository asesoriaRepository, IMapper mapper)
        {
            _asesoriaRepository = asesoriaRepository;
            _mapper = mapper;
        }

        public async Task<IList<AsesoriaModel>> GetAsesorias()
        {
            var asesorias = await _asesoriaRepository.GetAsesorias();
            var asesoriasOrdenadas = asesorias.OrderByDescending(a => a.FechaSolicitud).ToList();
            
            return asesoriasOrdenadas.Select((a, index) => new AsesoriaModel
            {
                Id = a.Id,
                UsuarioId = a.UsuarioId,
                UsuarioNombre = $"{a.Usuario.Nombre} {a.Usuario.Apellido}".Trim(),
                UsuarioEmail = a.Usuario.Email,
                Descripcion = a.Descripcion,
                FechaSolicitud = a.FechaSolicitud,
                FechaInicio = a.FechaInicio,
                FechaResolucion = a.FechaResolucion,
                Estado = a.Estado,
                Prioridad = a.Prioridad,
                MotivoRechazo = a.MotivoRechazo,
                Observaciones = a.Observaciones,
                AtendidoPorId = a.AtendidoPorId,
                AtendidoPorNombre = a.AtendidoPor != null ? $"{a.AtendidoPor.Nombre} {a.AtendidoPor.Apellido}".Trim() : null,
                NumeroTicket = index + 1
            }).ToList();
        }

        public async Task<AsesoriaModel?> GetAsesoria(Guid id)
        {
            var asesoria = await _asesoriaRepository.GetAsesoria(id);
            if (asesoria == null) return null;

            var todasLasAsesorias = await _asesoriaRepository.GetAsesorias();
            var ordenadas = todasLasAsesorias.OrderByDescending(a => a.FechaSolicitud).ToList();
            var indice = ordenadas.FindIndex(a => a.Id == id);

            return new AsesoriaModel
            {
                Id = asesoria.Id,
                UsuarioId = asesoria.UsuarioId,
                UsuarioNombre = $"{asesoria.Usuario.Nombre} {asesoria.Usuario.Apellido}".Trim(),
                UsuarioEmail = asesoria.Usuario.Email,
                Descripcion = asesoria.Descripcion,
                FechaSolicitud = asesoria.FechaSolicitud,
                FechaInicio = asesoria.FechaInicio,
                FechaResolucion = asesoria.FechaResolucion,
                Estado = asesoria.Estado,
                Prioridad = asesoria.Prioridad,
                MotivoRechazo = asesoria.MotivoRechazo,
                Observaciones = asesoria.Observaciones,
                AtendidoPorId = asesoria.AtendidoPorId,
                AtendidoPorNombre = asesoria.AtendidoPor != null ? $"{asesoria.AtendidoPor.Nombre} {asesoria.AtendidoPor.Apellido}".Trim() : null,
                NumeroTicket = indice >= 0 ? indice + 1 : 0
            };
        }

        public async Task<IList<AsesoriaModel>> GetAsesoriasByEstado(EstadoAsesoria estado)
        {
            var asesorias = await _asesoriaRepository.GetAsesoriasByEstado(estado);
            var asesoriasOrdenadas = asesorias.OrderByDescending(a => a.FechaSolicitud).ToList();

            return asesoriasOrdenadas.Select((a, index) => new AsesoriaModel
            {
                Id = a.Id,
                UsuarioId = a.UsuarioId,
                UsuarioNombre = $"{a.Usuario.Nombre} {a.Usuario.Apellido}".Trim(),
                UsuarioEmail = a.Usuario.Email,
                Descripcion = a.Descripcion,
                FechaSolicitud = a.FechaSolicitud,
                FechaInicio = a.FechaInicio,
                FechaResolucion = a.FechaResolucion,
                Estado = a.Estado,
                Prioridad = a.Prioridad,
                MotivoRechazo = a.MotivoRechazo,
                Observaciones = a.Observaciones,
                AtendidoPorId = a.AtendidoPorId,
                AtendidoPorNombre = a.AtendidoPor != null ? $"{a.AtendidoPor.Nombre} {a.AtendidoPor.Apellido}".Trim() : null,
                NumeroTicket = index + 1
            }).ToList();
        }

        public async Task<IList<AsesoriaModel>> GetAsesoriasByUsuarioId(Guid usuarioId)
        {
            var asesorias = await _asesoriaRepository.GetAsesoriasByUsuarioId(usuarioId);
            var asesoriasOrdenadas = asesorias.OrderByDescending(a => a.FechaSolicitud).ToList();

            return asesoriasOrdenadas.Select((a, index) => new AsesoriaModel
            {
                Id = a.Id,
                UsuarioId = a.UsuarioId,
                UsuarioNombre = $"{a.Usuario.Nombre} {a.Usuario.Apellido}".Trim(),
                UsuarioEmail = a.Usuario.Email,
                Descripcion = a.Descripcion,
                FechaSolicitud = a.FechaSolicitud,
                FechaInicio = a.FechaInicio,
                FechaResolucion = a.FechaResolucion,
                Estado = a.Estado,
                Prioridad = a.Prioridad,
                MotivoRechazo = a.MotivoRechazo,
                Observaciones = a.Observaciones,
                AtendidoPorId = a.AtendidoPorId,
                AtendidoPorNombre = a.AtendidoPor != null ? $"{a.AtendidoPor.Nombre} {a.AtendidoPor.Apellido}".Trim() : null,
                NumeroTicket = index + 1
            }).ToList();
        }

        public async Task<int> GetAsesoriasPendientesCount()
        {
            var asesorias = await _asesoriaRepository.GetAsesoriasByEstado(EstadoAsesoria.Pendiente);
            return asesorias.Count;
        }

        public async Task<int> GetAsesoriasEnProcesoCount()
        {
            var asesorias = await _asesoriaRepository.GetAsesoriasByEstado(EstadoAsesoria.EnProceso);
            return asesorias.Count;
        }

        public async Task<int> GetAsesoriasResueltasCount()
        {
            var asesorias = await _asesoriaRepository.GetAsesoriasByEstado(EstadoAsesoria.Resuelto);
            return asesorias.Count;
        }

        public async Task Create(AddAsesoriaModel model)
        {
            var asesoria = new Asesoria
            {
                Id = Guid.NewGuid(),
                UsuarioId = model.UsuarioId,
                Descripcion = model.Descripcion,
                Prioridad = model.Prioridad,
                FechaSolicitud = DateTime.UtcNow,
                Estado = EstadoAsesoria.Pendiente
            };

            await _asesoriaRepository.Save(asesoria);
        }

        public async Task AceptarAsesoria(Guid asesoriaId, Guid atendidoPorId)
        {
            var asesoria = await _asesoriaRepository.GetAsesoria(asesoriaId);
            if (asesoria == null)
            {
                throw new InvalidOperationException("Asesoría no encontrada.");
            }

            if (asesoria.Estado != EstadoAsesoria.Pendiente)
            {
                throw new InvalidOperationException("Solo se pueden aceptar asesorías pendientes.");
            }

            asesoria.Estado = EstadoAsesoria.EnProceso;
            asesoria.AtendidoPorId = atendidoPorId;
            asesoria.FechaInicio = DateTime.UtcNow;

            await _asesoriaRepository.Update(asesoria);
        }

        public async Task RechazarAsesoria(Guid asesoriaId, Guid rechazadoPorId, string motivoRechazo)
        {
            if (string.IsNullOrWhiteSpace(motivoRechazo))
            {
                throw new ArgumentException("El motivo del rechazo es obligatorio.", nameof(motivoRechazo));
            }

            var asesoria = await _asesoriaRepository.GetAsesoria(asesoriaId);
            if (asesoria == null)
            {
                throw new InvalidOperationException("Asesoría no encontrada.");
            }

            if (asesoria.Estado != EstadoAsesoria.Pendiente)
            {
                throw new InvalidOperationException("Solo se pueden rechazar asesorías pendientes.");
            }

            asesoria.Estado = EstadoAsesoria.Rechazado;
            asesoria.AtendidoPorId = rechazadoPorId;
            asesoria.MotivoRechazo = motivoRechazo.Trim();
            asesoria.FechaResolucion = DateTime.UtcNow;

            await _asesoriaRepository.Update(asesoria);
        }

        public async Task ResolverAsesoria(Guid asesoriaId, Guid resueltoPorId, string observaciones)
        {
            var asesoria = await _asesoriaRepository.GetAsesoria(asesoriaId);
            if (asesoria == null)
            {
                throw new InvalidOperationException("Asesoría no encontrada.");
            }

            if (asesoria.Estado != EstadoAsesoria.EnProceso)
            {
                throw new InvalidOperationException("Solo se pueden resolver asesorías en proceso.");
            }

            asesoria.Estado = EstadoAsesoria.Resuelto;
            asesoria.AtendidoPorId = resueltoPorId;
            asesoria.Observaciones = observaciones?.Trim();
            asesoria.FechaResolucion = DateTime.UtcNow;

            await _asesoriaRepository.Update(asesoria);
        }
    }
}

