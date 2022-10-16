using Funq;
using ServiceStack;
using ServiceStack.Admin;
using ServiceStack.Api.OpenApi;
using SlaytonNichols.Common.ServiceStack;
using SlaytonNichols.Services;
using System.Text.Encodings.Web;

[assembly: HostingStartup(typeof(SlaytonNichols.AppHost))]

namespace SlaytonNichols;

public class AppHost : AppHostBase, IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder.ConfigureApplication();

    public AppHost() : base("SlaytonNichols", typeof(PostEndpoint).Assembly) { }

    public override void Configure(Container container) { }
}
