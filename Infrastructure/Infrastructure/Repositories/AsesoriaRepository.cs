using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AsesoriaRepository : BaseRepository, IAsesoriaRepository
    {
        public AsesoriaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IList<Asesoria>> GetAsesorias()
        {
            return await context.Asesorias
                .Include(a => a.Usuario)
                .Include(a => a.AtendidoPor)
                .AsNoTracking()
                .OrderByDescending(a => a.FechaSolicitud)
                .ToListAsync();
        }

        public async Task<Asesoria?> GetAsesoria(Guid id)
        {
            return await context.Asesorias
                .Include(a => a.Usuario)
                .Include(a => a.AtendidoPor)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IList<Asesoria>> GetAsesoriasByEstado(Domain.Enums.EstadoAsesoria estado)
        {
            return await context.Asesorias
                .Include(a => a.Usuario)
                .Include(a => a.AtendidoPor)
                .Where(a => a.Estado == estado)
                .AsNoTracking()
                .OrderByDescending(a => a.FechaSolicitud)
                .ToListAsync();
        }

        public async Task<IList<Asesoria>> GetAsesoriasByUsuarioId(Guid usuarioId)
        {
            return await context.Asesorias
                .Include(a => a.Usuario)
                .Include(a => a.AtendidoPor)
                .Where(a => a.UsuarioId == usuarioId)
                .AsNoTracking()
                .OrderByDescending(a => a.FechaSolicitud)
                .ToListAsync();
        }

        public async Task Save(Asesoria asesoria)
        {
            await context.Asesorias.AddAsync(asesoria);
            await context.SaveChangesAsync();
        }

        public async Task Update(Asesoria asesoria)
        {
            try
            {
                await Begin();
                context.Asesorias.Update(asesoria);
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
                var asesoria = await context.Asesorias.FindAsync(id);
                if (asesoria != null)
                {
                    context.Asesorias.Remove(asesoria);
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


