using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class SolicitudPrestamoRepository : BaseRepository, ISolicitudPrestamoRepository
    {
        public SolicitudPrestamoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IList<SolicitudPrestamo>> GetSolicitudes()
        {
            return await context.SolicitudesPrestamo
                .Include(s => s.Usuario)
                .Include(s => s.Sala)
                .Include(s => s.Equipo)
                .Include(s => s.AprobadoPor)
                .AsNoTracking()
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();
        }

        public async Task<SolicitudPrestamo?> GetSolicitud(Guid id)
        {
            return await context.SolicitudesPrestamo
                .Include(s => s.Usuario)
                .Include(s => s.Sala)
                .Include(s => s.Equipo)
                .Include(s => s.AprobadoPor)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IList<SolicitudPrestamo>> GetSolicitudesByEstado(Domain.Enums.EstadoSolicitud estado)
        {
            return await context.SolicitudesPrestamo
                .Include(s => s.Usuario)
                .Include(s => s.Sala)
                .Include(s => s.Equipo)
                .Where(s => s.Estado == estado)
                .AsNoTracking()
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();
        }

        public async Task Save(SolicitudPrestamo solicitud)
        {
            await context.SolicitudesPrestamo.AddAsync(solicitud);
            await context.SaveChangesAsync();
        }

        public async Task Update(SolicitudPrestamo solicitud)
        {
            try
            {
                await Begin();
                context.SolicitudesPrestamo.Update(solicitud);
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
                var solicitud = await context.SolicitudesPrestamo.FindAsync(id);
                if (solicitud != null)
                {
                    context.SolicitudesPrestamo.Remove(solicitud);
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

