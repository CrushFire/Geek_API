using AutoMapper;
using Core.Models;
using Core.Models.Image;
using DataAccess.Entities;

DataAccess.Entities;

namespace Geek_API.Mappers;

public class AutoMapper : Profile
{
    public AutoMapper()
    {
        CreateMap<EntityUser, UserResponse>();
        CreateMap<UserRequest, EntityUser>()
            .ForMember(x => x.Id,
                opt => opt.Ignore()); //тут ничего не подтягиваем поэтому для создания и обновления одинаковый маппер

        CreateMap<EntityImage, ImageResponse>();
        CreateMap<ImageRespons
 EntityImage>();
    }
}