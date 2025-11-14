using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<IList<Usuario>> GetUsers();
        Task<Usuario?> GetUser(Guid id);
        Task<Usuario?> GetByEmail(string email);

        Task Save(Usuario usuario);
        Task Update(Usuario usuario);
    }
}
