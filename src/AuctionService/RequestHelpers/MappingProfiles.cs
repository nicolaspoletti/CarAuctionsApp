using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService.RequestHelpers;


class MappingProfiles : Profile
//using AutoMapper to define mapping profiles between different classes in the application.  
{
    public MappingProfiles()
    {
        CreateMap<Auction,AuctionDTO>().IncludeMembers(x => x.Item);
        CreateMap<Item ,AuctionDTO>();
        CreateMap<CreateAuctionDTO, Auction>().ForMember(d => d.Item, o => o.MapFrom(s => s));
        CreateMap<CreateAuctionDTO, Item>();
        CreateMap<AuctionDTO, AuctionCreated>();
        CreateMap<Auction, AuctionUpdated>().IncludeMembers(auct => auct.Item);
        CreateMap<Item, AuctionUpdated>();
    }
}