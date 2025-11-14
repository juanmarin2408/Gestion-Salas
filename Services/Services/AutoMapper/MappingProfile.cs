using AutoMapper;
using Domain;
using Services.Models.CowModels;
using Services.Models.FarmModels;
using Services.Models.MilkModels;
using Services.Models.UserModels;

namespace Services.Automapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            FarmMapper();
            MilkMapper();
            CowMapper();
            UserMapper();
        }

        private void MilkMapper()
        {
            CreateMap<Milk, MilkModel>()
            .ReverseMap();

        }

        private void CowMapper()
        {
            CreateMap<Cow, CowModel>()
            .ReverseMap();

            CreateMap<Cow, AddCowModel>()
            .ReverseMap();

        }

        private void FarmMapper()
        {
            CreateMap<Farm, FarmModel>()
                .ForMember(dest => dest.CowCount,
                           opt => opt.MapFrom(src => src.Cows != null ? src.Cows.Count : 0))
                .ForMember(dest => dest.TotalMilkLitters,opt => opt.MapFrom(src => src.getTotalLitters()))
            .ReverseMap();

            CreateMap<Farm, AddFarmModel>()
                .ReverseMap();
        }
        
        private void UserMapper()
        {
            // Usuario
            CreateMap<AddUserModel, Usuario>();
            CreateMap<Usuario, UserModel>();
        }

    }

    
}
