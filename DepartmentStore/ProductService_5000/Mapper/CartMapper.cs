using AutoMapper;
using ProductService_5000.Models;
using ProductService_5000.Response;

namespace ProductService_5000.Mapper
{
    public class CartMapper : Profile
    {
        public CartMapper() 
        {
            CreateMap<MRes_Cart,Cart>()
                .ForMember(dest => dest.IdProduct,opt=>opt.MapFrom(m=>m.IdProduct))
                .ForMember(dest => dest.IdBranch, opt => opt.MapFrom(m => m.IdBranch))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(m => m.Quantity));
        }
    }
}
