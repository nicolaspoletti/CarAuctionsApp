using MongoDB.Entities;
using SearchService;

public class AuctionSvcHttpClient
{
    public HttpClient _httpClient { get; }
    public IConfiguration _configuration { get; }
    
    
    
    
    public AuctionSvcHttpClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }


    public async Task<List<Item>> GetItems()
    {
        var lastUpdated = await DB.Find<Item, string>()
            .Sort(x => x.Descending(a => a.UpdatedAt))
            .Project(x => x.UpdatedAt.ToString())
            .ExecuteFirstAsync();

        return await _httpClient.GetFromJsonAsync<List<Item>>(_configuration["AuctionServiceUrl"] + "/api/auctions?date=" + lastUpdated); 
    }

}