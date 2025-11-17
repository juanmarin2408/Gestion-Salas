using Domain;
using Services.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IUserService
    {
        Task<IList<UserModel>> GetUsers();
        Task<UserModel?> GetUser(Guid id);
        Task<Usuario?> LoginAsync(string email, string password);
        Task Register(AddUserModel model);
        Task UpdateLastAccess(Guid userId);
        Task<bool> EmailExistsAsync(string email);
        Task ChangePasswordAsync(string email, string newPassword);
    }
}
