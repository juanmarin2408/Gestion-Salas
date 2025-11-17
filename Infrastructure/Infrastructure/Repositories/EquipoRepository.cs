using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class EquipoRepository : BaseRepository, IEquipoRepository
    {
        public EquipoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IList<Equipo>> GetEquipos()
        {
            return await context.Equipos
                .Include(e => e.Sala)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Equipo?> GetEquipo(Guid id)
        {
            return await context.Equipos
                .Include(e => e.Sala)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IList<Equipo>> GetEquiposBySalaId(Guid salaId)
        {
            return await context.Equipos
                .Where(e => e.SalaId == salaId)
                .Include(e => e.Sala)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IList<Equipo>> GetEquiposByEstado(Domain.Enums.EstadoEquipo estado)
        {
            return await context.Equipos
                .Where(e => e.Estado == estado)
                .Include(e => e.Sala)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task Save(Equipo equipo)
        {
            await context.Equipos.AddAsync(equipo);
            await context.SaveChangesAsync();
        }

        public async Task Update(Equipo equipo)
        {
            try
            {
                await Begin();
                context.Equipos.Update(equipo);
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
            var equipo = await GetEquipo(id);
            if (equipo != null)
            {
                try
                {
                    await Begin();
                    context.Equipos.Remove(equipo);
                    await Save();
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
}

