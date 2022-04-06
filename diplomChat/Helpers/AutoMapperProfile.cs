using AutoMapper;
using diplomChat.Entities;
using diplomChat.Models.Users;

namespace diplomChat.Helpers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, AuthenticateResponse>();
        CreateMap<RegisterRequest, User>();
        CreateMap<UpdateRequest, User>()
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    if (prop == null) return false;
                    return prop.GetType() != typeof(string) || !string.IsNullOrEmpty((string)prop);
                }
            ));
    }
}