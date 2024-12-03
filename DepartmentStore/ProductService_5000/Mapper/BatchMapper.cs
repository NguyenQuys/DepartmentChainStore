using AutoMapper;
using ProductService_5000.Models;
using ProductService_5000.Response;

namespace ProductService_5000.Mapper
{
    public class BatchMapper : Profile
    {
        public BatchMapper()
        {
            // Mapping for the update
            CreateMap<Batch, Batch>();

            CreateMap<Batch, MRes_Batch>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName));
        }
    }
}
