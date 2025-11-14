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
        Task Register(AddUserModel model);
    }
}
