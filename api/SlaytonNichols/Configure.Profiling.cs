using ServiceStack;

[assembly: HostingStartup(typeof(SlaytonNichols.ConfigureProfiling))]

namespace SlaytonNichols;

public class ConfigureProfiling : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureAppHost(host =>
        {
            host.Plugins.Add(new RequestLogsFeature
            {
                EnableResponseTracking = true,
            });

            host.Plugins.Add(new ProfilingFeature
            {
                IncludeStackTrace = true,
            });
        });
    }
}
