using AutoMapper;
using Kofti.Manager.Data.Entities;
using Kofti.Manager.Models;

namespace Kofti.Manager.Infrastructure
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ApplicationEntity, AppModel>().ReverseMap();
            CreateMap<ConfigEnttiy, ConfigModel>().ReverseMap();
        }
    }
}