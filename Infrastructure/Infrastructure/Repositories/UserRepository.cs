using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IList<Usuario>> GetUsers()
        {
            return await context.Usuarios
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Usuario?> GetUser(Guid id)
        {
            return await context.Usuarios
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Usuario?> GetByEmail(string email)
        {
            return await context.Usuarios
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<Usuario?> GetByDocumento(string documento)
        {
            return await context.Usuarios
                .FirstOrDefaultAsync(x => x.Documento == documento);
        }

        public async Task Save(Usuario usuario)
        {
            await context.Usuarios.AddAsync(usuario);
            await context.SaveChangesAsync();
        }

        public async Task Update(Usuario usuario)
        {
            try
            {
                await Begin();

                context.Usuarios.Update(usuario);

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
