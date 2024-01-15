using System.Text.Json;
using Microsoft.VisualBasic;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService;

class DbInitializer
{
    public static async Task InitDb(WebApplication app) 
    {
        await DB.InitAsync("SearchDB", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();

        var count = await DB.CountAsync<Item>();

        using var scope = app.Services.CreateScope();

        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();

        var items = await httpClient.GetItems();

        System.Console.WriteLine(items.Count + "returned from the Auction Service");

        if (items.Count > 0) await DB.SaveAsync(items);
    }
}