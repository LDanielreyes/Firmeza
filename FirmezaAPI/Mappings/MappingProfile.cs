using AutoMapper;
using Firmeza.Data.Entities;
using FirmezaAPI.DTOs;

namespace FirmezaAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            
            CreateMap<Client, ClientDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<Sale, SaleDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            CreateMap<Receipt, ReceiptDto>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.FullName));
        }
    }
}
