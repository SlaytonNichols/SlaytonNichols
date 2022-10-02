using Funq;
using ServiceStack;
using ServiceStack.Api.OpenApi;
using SlaytonNichols.ServiceInterface;

using System.Text.Encodings.Web;

[assembly: HostingStartup(typeof(SlaytonNichols.AppHost))]

namespace SlaytonNichols;

public class AppHost : AppHostBase, IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) =>
        {
            services.ConfigureNonBreakingSameSiteCookies(context.HostingEnvironment);
        }).ConfigureLogging(logginBuilder =>
        {
            logginBuilder.ClearProviders();
            logginBuilder.AddJsonConsole(jsonConsoleFormatterOptions =>
            {
                jsonConsoleFormatterOptions.JsonWriterOptions = new()
                {
                    Indented = false,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
            });
        });

    public AppHost() : base("SlaytonNichols", typeof(PostsServices).Assembly) { }

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


        Plugins.Add(new CorsFeature(allowOriginWhitelist: new[]{
            "http://localhost:5000",
            "http://localhost:3000",
            "http://localhost:5173",
            "https://localhost:5001",
            "https://" + Environment.GetEnvironmentVariable("DEPLOY_CDN")
        }, allowCredentials: true));


        ConfigurePlugin<UiFeature>(feature =>
        {
            feature.Info.BrandIcon.Uri = "/assets/img/logo.svg";
            feature.Info.BrandIcon.Cls = "inline-block w-8 h-8 mr-2";
        });
    }
}
