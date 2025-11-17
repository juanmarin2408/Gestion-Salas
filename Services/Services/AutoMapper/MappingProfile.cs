using AutoMapper;
using Domain;
using Services.Models.EquipoModels;
using Services.Models.SalaModels;
using Services.Models.UserModels;

namespace Services.Automapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            UserMapper();
            SalaMapper();
            EquipoMapper();
        }
        
        private void UserMapper()
        {
            // Usuario
            CreateMap<AddUserModel, Usuario>();
            CreateMap<Usuario, UserModel>();
        }

        private void SalaMapper()
        {
            // Sala
            CreateMap<AddSalaModel, Sala>();
            CreateMap<Sala, SalaModel>();
        }

        private void EquipoMapper()
        {
            // Equipo
            CreateMap<AddEquipoModel, Equipo>();
            CreateMap<Equipo, EquipoModel>();
        }
    }
}
