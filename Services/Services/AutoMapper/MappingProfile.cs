using AutoMapper;
using Domain;
using Services.Models.UserModels;

namespace Services.Automapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            UserMapper();
        }
        
        private void UserMapper()
        {
            // Usuario
            CreateMap<AddUserModel, Usuario>();
            CreateMap<Usuario, UserModel>();
        }

    }

    
}
