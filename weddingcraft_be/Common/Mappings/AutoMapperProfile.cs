using AutoMapper;
using weddingcraft_be.Dtos;
using weddingcraft_be.Models;

namespace weddingcraft_be.Common.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User mappings
            CreateMap<User, UserDto>();
            CreateMap<VendorProfile, VendorProfileDto>().ReverseMap();

            // Product mappings
            CreateMap<Product, ProductDto>().ReverseMap();


        }
    }
}
