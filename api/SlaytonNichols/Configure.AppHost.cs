using Funq;
using SlaytonNichols.Common.ServiceStack;
using SlaytonNichols.Controllers;

[assembly: HostingStartup(typeof(SlaytonNichols.AppHost))]

namespace SlaytonNichols;

public class AppHost : AppHostBase, IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder.ConfigureApplication();

    public AppHost() : base("SlaytonNichols", typeof(PostsController).Assembly) { }

    public override void Configure(Container container) { }
}
