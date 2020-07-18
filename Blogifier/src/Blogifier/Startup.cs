using Blogifier.Core;
using Blogifier.Core.Extensions;
using Blogifier.Widgets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Serilog;
using Serilog.Events;
using Sotsera.Blazor.Toaster.Core.Models;
using System;
using System.Linq;

namespace Blogifier
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
              .WriteTo.RollingFile("Logs/{Date}.txt", LogEventLevel.Warning)
              .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddBlogDatabase();
            services.AddBlogSecurity();
            services.AddBlogLocalization();

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            services.AddFeatureManagement().AddFeatureFilter<EmailFeatureFilter>();

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllersWithViews().AddViewLocalization();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            services.AddRazorPages(options => 
                options.Conventions.AuthorizeFolder("/Admin")
                .AllowAnonymousToPage("/Admin/_Host")
            ).AddRazorRuntimeCompilation().AddViewLocalization();

            services.AddServerSideBlazor();

            //services.AddHttpContextAccessor();
            
            services.AddToaster(config =>
            {
                config.PositionClass = Defaults.Classes.Position.BottomRight;
                config.PreventDuplicates = true;
                config.NewestOnTop = false;
            });

            services.AddBlogServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Console.WriteLine("HostingEnvironmentName: '{0}'", env.EnvironmentName);
            app.UseResponseCompression();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            AppSettings.WebRootPath = env.WebRootPath;
            AppSettings.ContentRootPath = env.ContentRootPath;
            AppSettings.ThumbWidth = 270;
            AppSettings.ThumbHeight = 180;

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseRequestLocalization();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Blog}/{action=Index}/{id?}"
                );
                endpoints.MapRazorPages();
                endpoints.MapBlazorHub();
                endpoints.MapHub<MainHub>("/mainhub");
                endpoints.MapFallbackToPage("/Admin/_Host");
            });
        }
    }
}