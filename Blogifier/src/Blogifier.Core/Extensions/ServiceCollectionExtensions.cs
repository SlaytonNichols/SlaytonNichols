using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Blogifier.Core.Data;
using Blogifier.Core.Data.Repositories;
using Blogifier.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Blogifier.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBlogSettings<T>(this IServiceCollection services, IConfigurationSection section) where T : class, new()
        {
            services.Configure<T>(section);
            services.AddTransient<IAppService<T>>(provider =>
            {
                var options = provider.GetService<IOptionsMonitor<T>>();
                return new AppService<T>(options);
            });
        }

        public static IServiceCollection AddBlogServices(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IDataService, DataService>();
            services.AddTransient<IFeedService, FeedService>();
            services.AddTransient<IStorageService, StorageService>();
            services.AddTransient<IImportService, ImportService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IWebService, WebService>();
            services.AddTransient<IEmailService, SendGridService>();

            services.AddTransient<UserManager<AppUser>>();

            AddBlogRepositories(services);

            return services;
        }

        public static IServiceCollection AddBlogDatabase(this IServiceCollection services)
        {           
            var connectionString = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Production ? Environment.GetEnvironmentVariable("CONNECTION_STRING") : Environment.GetEnvironmentVariable("TEST_CONNECTION_STRING") ?? throw new Exception("Test Connection String Not Configured");

            AppSettings.DbOptions = options => options.UseSqlServer(connectionString);

            services.AddDbContext<AppDbContext>(AppSettings.DbOptions, ServiceLifetime.Scoped);


            return services;
        }

        public static IServiceCollection AddBlogLocalization(this IServiceCollection services)
        {
            var supportedCultures = new HashSet<CultureInfo>()
            {
                new CultureInfo("en-US"),
                new CultureInfo("es-ES"),
                new CultureInfo("pt-BR"),
                new CultureInfo("ru-RU"),
                new CultureInfo("zh-cn"),
                new CultureInfo("zh-tw")
            };

            services.AddJsonLocalization(options => {
                options.DefaultCulture = new CultureInfo("en-US");
                options.ResourcesPath = "Resources";
                options.SupportedCultureInfos = supportedCultures;
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures = supportedCultures.ToArray();
                options.SupportedUICultures = supportedCultures.ToArray();
            });

            return services;
        }

        public static IServiceCollection AddBlogSecurity(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.AllowedUserNameCharacters = null;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            return services;
        }

        private static void AddBlogRepositories(IServiceCollection services)
        {
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<ICustomFieldRepository, CustomFieldRepository>();            
            services.AddScoped<INewsletterRepository, NewsletterRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();           
                      
        }
    }
}