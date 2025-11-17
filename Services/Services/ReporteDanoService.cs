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
        private readonly IMapper _mapper;

        public ReporteDanoService(IReporteDanoRepository reporteRepository, IMapper mapper)
        {
            _reporteRepository = reporteRepository;
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
                EquipoId = r.EquipoId,
                EquipoCodigo = r.Equipo.Id.ToString().Substring(0, 8).ToUpper(),
                SalaId = r.Equipo.SalaId,
                SalaNumero = r.Equipo.Sala.Numero,
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
                EquipoId = reporte.EquipoId,
                EquipoCodigo = reporte.Equipo.Id.ToString().Substring(0, 8).ToUpper(),
                SalaId = reporte.Equipo.SalaId,
                SalaNumero = reporte.Equipo.Sala.Numero,
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
                EquipoId = r.EquipoId,
                EquipoCodigo = r.Equipo.Id.ToString().Substring(0, 8).ToUpper(),
                SalaId = r.Equipo.SalaId,
                SalaNumero = r.Equipo.Sala.Numero,
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
    }
}

