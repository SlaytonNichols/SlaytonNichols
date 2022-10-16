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

    public override void Configure(Container container)
    {
        Plugins.Add(new OpenApiFeature());
        Plugins.Add(new PostmanFeature());
        //ServiceStack
        SetConfig(new HostConfig
        {
        });
        Plugins.Add(new SpaFeature
        {
            EnableSpaFallback = true
        });
        ConfigurePlugin<UiFeature>(feature =>
        {
        });
        Plugins.Add(new AdminUsersFeature());


        Plugins.Add(new CorsFeature(allowOriginWhitelist: new[]{
            "http://localhost:5000",
            "http://localhost:3000",
            "http://localhost:5173",
            "https://localhost:5001",
            "https://" + Environment.GetEnvironmentVariable("DEPLOY_CDN")
        }, allowCredentials: true));
    }
}
