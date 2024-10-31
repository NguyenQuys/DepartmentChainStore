using AutoMapper;
using UserService_5002.Models;
using UserService_5002.Request;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<MReq_Staff, UserOtherInfo>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.NumberOfIncorrectEntries, opt => opt.Ignore())
            .ForMember(dest => dest.LoginTime, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.LogoutTime, opt => opt.Ignore());
    }
}

