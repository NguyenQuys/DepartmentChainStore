using AutoMapper;
using BranchService_5003.Models;
using BranchService_5003.Response;

namespace BranchService_5003.Mapper
{
    public class Product_BranchMapper : Profile
    {
        public Product_BranchMapper() {
            CreateMap<ImportProductHistory, MRes_ImportProductHistory>()
               .ForMember(dest => dest.LocationBranch, opt => opt.MapFrom(src => src.Branch.Location));
        }
    }
}
