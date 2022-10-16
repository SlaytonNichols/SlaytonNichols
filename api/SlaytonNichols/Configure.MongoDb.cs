using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

[assembly: HostingStartup(typeof(SlaytonNichols.ConfigureMongoDb))]

namespace SlaytonNichols
{
    public class ConfigureMongoDb : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder) => builder
            .ConfigureServices((context, services) =>
            {
                var mongoClient = new MongoClient(context.Configuration.GetConnectionString("Mongo"));
                IMongoDatabase mongoDatabase = mongoClient.GetDatabase("SlaytonNichols");
                services.AddSingleton(mongoDatabase);
            });
    }
}