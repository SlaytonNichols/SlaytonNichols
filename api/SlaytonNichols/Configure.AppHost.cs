using Funq;
using ServiceStack;
using SlaytonNichols.ServiceInterface;
using ServiceStack.Logging.Log4Net;
using ServiceStack.Logging;

[assembly: HostingStartup(typeof(SlaytonNichols.AppHost))]

namespace SlaytonNichols;

public class AppHost : AppHostBase, IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) => {
            services.ConfigureNonBreakingSameSiteCookies(context.HostingEnvironment);
        });

    public AppHost() : base("SlaytonNichols", typeof(MyServices).Assembly, typeof(TodosServices).Assembly) {}

    public override void Configure(Container container)
    {
        //Logging
        LogManager.LogFactory = new Log4NetFactory("log4net.config");
        container.Register<ILog>(ctx => LogManager.LogFactory.GetLogger(typeof(IService)));

        //ServiceStack
        SetConfig(new HostConfig {
        });


        Plugins.Add(new SpaFeature {
            EnableSpaFallback = true
        });


        Plugins.Add(new CorsFeature(allowOriginWhitelist:new[]{ 
            "http://localhost:5000",
            "http://localhost:3000",
            "http://localhost:5173",
            "https://localhost:5001",
            "https://" + Environment.GetEnvironmentVariable("DEPLOY_CDN")
        }, allowCredentials:true));


        ConfigurePlugin<UiFeature>(feature => {
            feature.Info.BrandIcon.Uri = "/assets/img/logo.svg";
            feature.Info.BrandIcon.Cls = "inline-block w-8 h-8 mr-2";
        });
    }
}
