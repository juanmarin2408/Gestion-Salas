using AutoMapper;
using Domain;
using Domain.Enums;
using Infrastructure.Repositories;
using Services.Models.ReporteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class ReporteDanoService : IReporteDanoService
    {
        private readonly IReporteDanoRepository _reporteRepository;
        private readonly IEquipoRepository _equipoRepository;
        private readonly IMapper _mapper;

        public ReporteDanoService(
            IReporteDanoRepository reporteRepository, 
            IEquipoRepository equipoRepository,
            IMapper mapper)
        {
            _reporteRepository = reporteRepository;
            _equipoRepository = equipoRepository;
            _mapper = mapper;
        }

        public async Task<IList<ReporteModel>> GetReportes()
        {
            var reportes = await _reporteRepository.GetReportes();
            return reportes.Select(r => new ReporteModel
            {
                Id = r.Id,
                UsuarioId = r.UsuarioId,
                UsuarioNombre = $"{r.Usuario.Nombre} {r.Usuario.Apellido}".Trim(),
                UsuarioEmail = r.Usuario.Email,
                Tipo = r.Tipo,
                EquipoId = r.EquipoId,
                EquipoCodigo = r.Equipo != null ? r.Equipo.Id.ToString().Substring(0, 8).ToUpper() : null,
                SalaId = r.SalaId, // Ya está guardado en el reporte (del equipo o directamente)
                SalaNumero = r.Sala != null ? r.Sala.Numero : (r.Equipo != null ? r.Equipo.Sala?.Numero : null),
                Descripcion = r.Descripcion,
                FechaReporte = r.FechaReporte,
                FechaResolucion = r.FechaResolucion,
                Estado = r.Estado,
                Prioridad = r.Prioridad,
                Observaciones = r.Observaciones,
                ResueltoPorId = r.ResueltoPorId,
                ResueltoPorNombre = r.ResueltoPor != null ? $"{r.ResueltoPor.Nombre} {r.ResueltoPor.Apellido}".Trim() : null
            }).ToList();
        }

        public async Task<ReporteModel?> GetReporte(Guid id)
        {
            var reporte = await _reporteRepository.GetReporte(id);
            if (reporte == null) return null;

            return new ReporteModel
            {
                Id = reporte.Id,
                UsuarioId = reporte.UsuarioId,
                UsuarioNombre = $"{reporte.Usuario.Nombre} {reporte.Usuario.Apellido}".Trim(),
                UsuarioEmail = reporte.Usuario.Email,
                Tipo = reporte.Tipo,
                EquipoId = reporte.EquipoId,
                EquipoCodigo = reporte.Equipo != null ? reporte.Equipo.Id.ToString().Substring(0, 8).ToUpper() : null,
                SalaId = reporte.SalaId, // Ya está guardado en el reporte (del equipo o directamente)
                SalaNumero = reporte.Sala != null ? reporte.Sala.Numero : (reporte.Equipo != null ? reporte.Equipo.Sala?.Numero : null),
                Descripcion = reporte.Descripcion,
                FechaReporte = reporte.FechaReporte,
                FechaResolucion = reporte.FechaResolucion,
                Estado = reporte.Estado,
                Prioridad = reporte.Prioridad,
                Observaciones = reporte.Observaciones,
                ResueltoPorId = reporte.ResueltoPorId,
                ResueltoPorNombre = reporte.ResueltoPor != null ? $"{reporte.ResueltoPor.Nombre} {reporte.ResueltoPor.Apellido}".Trim() : null
            };
        }

        public async Task<IList<ReporteModel>> GetReportesByEstado(EstadoReporte estado)
        {
            var reportes = await _reporteRepository.GetReportesByEstado(estado);
            return reportes.Select(r => new ReporteModel
            {
                Id = r.Id,
                UsuarioId = r.UsuarioId,
                UsuarioNombre = $"{r.Usuario.Nombre} {r.Usuario.Apellido}".Trim(),
                UsuarioEmail = r.Usuario.Email,
                Tipo = r.Tipo,
                EquipoId = r.EquipoId,
                EquipoCodigo = r.Equipo != null ? r.Equipo.Id.ToString().Substring(0, 8).ToUpper() : null,
                SalaId = r.SalaId, // Ya está guardado en el reporte (del equipo o directamente)
                SalaNumero = r.Sala != null ? r.Sala.Numero : (r.Equipo != null ? r.Equipo.Sala?.Numero : null),
                Descripcion = r.Descripcion,
                FechaReporte = r.FechaReporte,
                FechaResolucion = r.FechaResolucion,
                Estado = r.Estado,
                Prioridad = r.Prioridad,
                Observaciones = r.Observaciones,
                ResueltoPorId = r.ResueltoPorId,
                ResueltoPorNombre = r.ResueltoPor != null ? $"{r.ResueltoPor.Nombre} {r.ResueltoPor.Apellido}".Trim() : null
            }).ToList();
        }

        public async Task<int> GetReportesPendientesCount()
        {
            var reportes = await _reporteRepository.GetReportesByEstado(EstadoReporte.Pendiente);
            return reportes.Count;
        }

        public async Task ResolverReporte(Guid reporteId, Guid resueltoPorId, string observaciones)
        {
            var reporte = await _reporteRepository.GetReporte(reporteId);
            if (reporte == null)
            {
                throw new InvalidOperationException("Reporte no encontrado.");
            }

            if (reporte.Estado == EstadoReporte.Resuelto || reporte.Estado == EstadoReporte.Rechazado)
            {
                throw new InvalidOperationException("El reporte ya ha sido resuelto o rechazado.");
            }

            reporte.Estado = EstadoReporte.Resuelto;
            reporte.FechaResolucion = DateTime.UtcNow;
            reporte.ResueltoPorId = resueltoPorId;
            reporte.Observaciones = observaciones;

            await _reporteRepository.Update(reporte);
        }

        public async Task Create(AddReporteModel model)
        {
            // Validar que se proporcione el ID correspondiente según el tipo
            if (model.Tipo == TipoReporte.Equipo && !model.EquipoId.HasValue)
            {
                throw new InvalidOperationException("El ID del equipo es requerido para reportes de equipo.");
            }

            if (model.Tipo == TipoReporte.Sala && !model.SalaId.HasValue)
            {
                throw new InvalidOperationException("El ID de la sala es requerido para reportes de infraestructura de sala.");
            }

            Guid? salaId = null;

            // Si es reporte de equipo, obtener el SalaId del equipo
            if (model.Tipo == TipoReporte.Equipo && model.EquipoId.HasValue)
            {
                var equipo = await _equipoRepository.GetEquipo(model.EquipoId.Value);
                if (equipo == null)
                {
                    throw new InvalidOperationException("El equipo especificado no existe.");
                }
                salaId = equipo.SalaId; // Guardar también el SalaId del equipo
            }
            // Si es reporte de sala, usar el SalaId proporcionado
            else if (model.Tipo == TipoReporte.Sala && model.SalaId.HasValue)
            {
                salaId = model.SalaId.Value;
            }

            var reporte = new ReporteDano
            {
                Id = Guid.NewGuid(),
                UsuarioId = model.UsuarioId,
                Tipo = model.Tipo,
                EquipoId = model.Tipo == TipoReporte.Equipo ? model.EquipoId : null,
                SalaId = salaId, // Siempre guardar el SalaId (del equipo o directamente)
                Descripcion = model.Descripcion,
                Prioridad = model.Prioridad,
                FechaReporte = DateTime.UtcNow,
                Estado = EstadoReporte.Pendiente
            };

            await _reporteRepository.Save(reporte);
        }
    }
}

