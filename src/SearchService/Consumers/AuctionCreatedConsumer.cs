using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService;

/*
AuctionCreatedConsumer class will have methods defined by the IConsumer interface 
that dictate how it should handle messages of the AuctionCreated type.
*/
public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    private readonly IMapper _mapper;

    public AuctionCreatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }
    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        Console.WriteLine("--> Consuming AuctionCreated: " + context.Message.Id);
        
        var item = _mapper.Map<Item>(context.Message);


        if (item.Model == "Foo") throw new ArgumentException("Cannot sell cars with the name of Foo");

        await item.SaveAsync();
    }
}