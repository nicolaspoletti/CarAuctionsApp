using AuctionService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); 
builder.Services.AddDbContext<AuctionDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());




var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("nixoooooooooo");
try
{
    DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
}
app.MapGet("/", () => "Hello World!");
app.Run();
