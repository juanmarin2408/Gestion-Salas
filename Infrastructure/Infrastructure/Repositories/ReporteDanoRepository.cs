using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ReporteDanoRepository : BaseRepository, IReporteDanoRepository
    {
        public ReporteDanoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IList<ReporteDano>> GetReportes()
        {
            return await context.ReportesDanos
                .Include(r => r.Usuario)
                .Include(r => r.Equipo)
                    .ThenInclude(e => e.Sala)
                .Include(r => r.ResueltoPor)
                .AsNoTracking()
                .OrderByDescending(r => r.FechaReporte)
                .ToListAsync();
        }

        public async Task<ReporteDano?> GetReporte(Guid id)
        {
            return await context.ReportesDanos
                .Include(r => r.Usuario)
                .Include(r => r.Equipo)
                    .ThenInclude(e => e.Sala)
                .Include(r => r.ResueltoPor)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IList<ReporteDano>> GetReportesByEstado(Domain.Enums.EstadoReporte estado)
        {
            return await context.ReportesDanos
                .Include(r => r.Usuario)
                .Include(r => r.Equipo)
                    .ThenInclude(e => e.Sala)
                .Where(r => r.Estado == estado)
                .AsNoTracking()
                .OrderByDescending(r => r.FechaReporte)
                .ToListAsync();
        }

        public async Task Save(ReporteDano reporte)
        {
            await context.ReportesDanos.AddAsync(reporte);
            await context.SaveChangesAsync();
        }

        public async Task Update(ReporteDano reporte)
        {
            try
            {
                await Begin();
                context.ReportesDanos.Update(reporte);
                await Save();
                await Commit();
            }
            catch (Exception)
            {
                await RollBack();
                throw;
            }
        }

        public async Task Delete(Guid id)
        {
            try
            {
                await Begin();
                var reporte = await context.ReportesDanos.FindAsync(id);
                if (reporte != null)
                {
                    context.ReportesDanos.Remove(reporte);
                    await Save();
                }
                await Commit();
            }
            catch (Exception)
            {
                await RollBack();
                throw;
            }
        }
    }
}

