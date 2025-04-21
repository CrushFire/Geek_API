using AutoMapper;
using Core.Models;
using DataAccess.Entities;

namespace Geek_API.Mappers
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<EntityUser, DtoResponseUser>();
            CreateMap<DtoRequestUser, EntityUser>()
                .ForMember(x => x.Id, opt => opt.Ignore());//тут ничего не подтягиваем поэтому для создания и обновления одинаковый маппер

            CreateMap<EntityImage, DtoResponseImage>();
            CreateMap<DtoResponseImage, EntityImage>();
        }
    }
}
