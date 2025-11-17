using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class SalaRepository : BaseRepository, ISalaRepository
    {
        public SalaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IList<Sala>> GetSalas()
        {
            return await context.Salas
                .Include(s => s.Usuario)
                .Include(s => s.Equipos)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Sala?> GetSala(Guid id)
        {
            return await context.Salas
                .Include(s => s.Usuario)
                .Include(s => s.Equipos)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Sala?> GetSalaByNumero(string numero)
        {
            return await context.Salas
                .FirstOrDefaultAsync(x => x.Numero == numero);
        }

        public async Task<IList<Sala>> GetSalasByEstado(Domain.Enums.EstadoSala estado)
        {
            return await context.Salas
                .Where(s => s.Estado == estado)
                .Include(s => s.Equipos)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IList<Sala>> GetSalasByUsuarioId(Guid usuarioId)
        {
            return await context.Salas
                .Where(s => s.UsuarioId == usuarioId)
                .Include(s => s.Equipos)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task Save(Sala sala)
        {
            await context.Salas.AddAsync(sala);
            await context.SaveChangesAsync();
        }

        public async Task Update(Sala sala)
        {
            try
            {
                await Begin();
                context.Salas.Update(sala);
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
            var sala = await GetSala(id);
            if (sala != null)
            {
                try
                {
                    await Begin();
                    context.Salas.Remove(sala);
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

