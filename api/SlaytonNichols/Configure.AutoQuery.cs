using ServiceStack;
using ServiceStack.Data;
using SlaytonNichols.ServiceInterface;
using SlaytonNichols.ServiceModel;

[assembly: HostingStartup(typeof(SlaytonNichols.ConfigureAutoQuery))]

namespace SlaytonNichols;

public class ConfigureAutoQuery : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(services =>
        {
            // Enable Audit History
            services.AddSingleton<ICrudEvents>(c =>
                new OrmLiteCrudEvents(c.Resolve<IDbConnectionFactory>()));
        })
        .ConfigureAppHost(appHost =>
        {
            appHost.Plugins.Add(new AutoQueryFeature
            {
                MaxLimit = 1000,
                //IncludeTotal = true,
            });

            appHost.Resolve<ICrudEvents>().InitSchema();
        });
}
