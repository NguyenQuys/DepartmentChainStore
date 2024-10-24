using AutoMapper;
using ProductService_5000.Models;
using ProductService_5000.Request;

namespace ProductService_5000.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<MReq_Product, Product>()
                .ForMember(dest => dest.UpdatedTime, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.IsHide, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Images, opt => opt.Ignore());  // Bỏ qua ánh xạ cho Images tại đây
        }
    }


}
