using AutoMapper;
using Contracts;
using SearchService;

namespace SearchService;

class MappingProfiles : Profile
{
    public MappingProfiles(){
        CreateMap<AuctionCreated, Item>();
        CreateMap<AuctionUpdated, Item>();
    }
}