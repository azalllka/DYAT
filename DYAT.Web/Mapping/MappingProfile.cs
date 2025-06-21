using AutoMapper;
using DYAT.Domain.Entities;
using DYAT.Web.Areas.CreateNft.Models;
using DYAT.Web.Areas.Nfts.Models;

namespace DYAT.Web.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateNftViewModel, Nft>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

        CreateMap<Nft, NftViewModel>()
            .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator.UserName))
            .ForMember(dest => dest.HighestBid, opt => opt.MapFrom(src => src.Price)) // Временно используем цену как высшую ставку
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => 5)); // Временно используем фиксированный рейтинг
    }
} 