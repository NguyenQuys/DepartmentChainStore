using AutoMapper;
using ProductService_5000.Models;
using ProductService_5000.Request;
using ProductService_5000.Response;

namespace ProductService_5000.Mapper
{
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            CreateMap<MReq_Product, Product>()
                .ForMember(dest => dest.UpdatedTime, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.IsHide, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Images, opt => opt.Ignore());  // Bỏ qua ánh xạ cho Images tại đây

            CreateMap<Product, MRes_Product>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(opt => opt.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(m => m.ProductName))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(m => m.Price))
                .ForMember(dest => dest.MainImage, opt => opt.MapFrom(m => m.MainImage));
        }
    }
}
