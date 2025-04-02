using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

public class DbInitilizer
{
    public static async Task InitDB(WebApplication app)
    {
        await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDBConnection")));

        await DB.Index<Item>()
            .Key(a => a.Make, KeyType.Text)
            .Key(a => a.Model, KeyType.Text)
            .Key(a => a.Year, KeyType.Text)
            .Key(a => a.Color, KeyType.Text)
            .Key(a => a.Mileage, KeyType.Text)
            .Key(a => a.Status, KeyType.Text)
            .Key(a => a.CreatedAt, KeyType.Text)
            .Key(a => a.UpdatedAt, KeyType.Text)
            .Key(a => a.AuctionEnd, KeyType.Text)
            .Key(a => a.ImageUrl, KeyType.Text)
            .CreateAsync();

        var count = await DB.CountAsync<Item>();
        
         using var scope = app.Services.CreateAsyncScope();
        var auctionService = scope.ServiceProvider.GetRequiredService<AuctionServiceHttp>();
        
        var items = await auctionService.GetItemsForSeachDb();

        Console.WriteLine($"Items count: {items.Count}");

        if (items.Count > 0) await DB.SaveAsync(items);
    }
}