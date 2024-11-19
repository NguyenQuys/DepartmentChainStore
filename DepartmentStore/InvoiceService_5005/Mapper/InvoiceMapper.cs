using AutoMapper;
using InvoiceService_5005.InvoiceModels;
using InvoiceService_5005.Request;

namespace InvoiceService_5005.NewFolder
{
    public class InvoiceMapper : Profile
    {
        public InvoiceMapper()
        {
            CreateMap<MReq_Invoice, Invoice>()
                .ForMember(dest => dest.IdBranch, opt => opt.MapFrom(src => src.IdBranch))
                .ForMember(dest => dest.CustomerPhoneNumber, opt => opt.MapFrom(src => src.CustomerPhoneNumber))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.IdStatus, opt => opt.MapFrom(_ => 1))
                .ForMember(dest => dest.IdPaymentMethod, opt => opt.MapFrom(src => src.IdPaymentMethod));
        }
    }
}
