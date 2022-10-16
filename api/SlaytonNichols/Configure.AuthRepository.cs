using System;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Configuration;
using ServiceStack.Authentication.MongoDb;
using MongoDB.Driver;
using ServiceStack.Web;

[assembly: HostingStartup(typeof(SlaytonNichols.ConfigureAuthRepository))]

namespace SlaytonNichols
{
    // Custom User Table with extended Metadata properties
    public class AppUser : UserAuth
    {
    }

    public class AppUserAuthEvents : AuthEvents
    {
        public override void OnAuthenticated(IRequest req, IAuthSession session, IServiceBase authService,
            IAuthTokens tokens, Dictionary<string, string> authInfo)
        {
            var authRepo = HostContext.AppHost.GetAuthRepository(req);
            using (authRepo as IDisposable)
            {
                var userAuth = authRepo.GetUserAuth(session.UserAuthId);
                authRepo.SaveUserAuth(userAuth);
            }
        }
    }

    public class ConfigureAuthRepository : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder) => builder
            .ConfigureServices(services => services.AddSingleton<IAuthRepository>(c =>
                new MongoDbAuthRepository(c.Resolve<IMongoDatabase>(), createMissingCollections: true)))
            .ConfigureAppHost(appHost =>
            {
                var authRepo = appHost.Resolve<IAuthRepository>();
                authRepo.InitSchema();
                // CreateUser(authRepo, "admin@email.com", "Admin User", "p@55wOrd", roles: new[] { RoleNames.Admin });
            }, afterConfigure: appHost =>
                appHost.AssertPlugin<AuthFeature>().AuthEvents.Add(new AppUserAuthEvents()));

        // Add initial Users to the configured Auth Repository
        // public void CreateUser(IAuthRepository authRepo, string email, string name, string password, string[] roles)
        // {
        //     if (authRepo.GetUserAuthByUserName(email) == null)
        //     {
        //         var newAdmin = new AppUser { Email = email, DisplayName = name };
        //         var user = authRepo.CreateUserAuth(newAdmin, password);
        //         authRepo.AssignRoles(user, roles);
        //     }
        // }
    }
}
