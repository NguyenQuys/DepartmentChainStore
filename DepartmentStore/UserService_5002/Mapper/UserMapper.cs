using AutoMapper;
using UserService_5002.Models;
using UserService_5002.Request;
using UserService_5002.Response;

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

        CreateMap<User, MReq_Staff>()
            .ForMember(dest => dest.IdUser, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.UserOtherInfo.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserOtherInfo.Email))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.UserOtherInfo.DateOfBirth))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.UserOtherInfo.Gender))
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.UserOtherInfo.RoleId))
            .ForMember(dest => dest.IdBranch, opt => opt.MapFrom(src => src.UserOtherInfo.IdBranch))
            .ForMember(dest => dest.BeginDate, opt => opt.MapFrom(src => src.UserOtherInfo.BeginDate))
            .ForMember(dest => dest.Salary, opt => opt.MapFrom(src => src.UserOtherInfo.Salary));

        CreateMap<User, MRes_Customer>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.UserOtherInfo.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserOtherInfo.Email))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.UserOtherInfo.DateOfBirth))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.UserOtherInfo.Gender))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UserOtherInfo.UpdatedAt))
            .ForMember(dest => dest.LoginTime, opt => opt.MapFrom(src => src.UserOtherInfo.LoginTime))
            .ForMember(dest => dest.LogoutTime, opt => opt.MapFrom(src => src.UserOtherInfo.LogoutTime));
    }
}

