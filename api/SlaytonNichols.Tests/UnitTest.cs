using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.Host;
using ServiceStack.OrmLite;
using ServiceStack.Testing;
using SlaytonNichols.ServiceInterface;
using SlaytonNichols.ServiceModel;

namespace SlaytonNichols.Tests
{
    public class UnitTest
    {
        private readonly ServiceStackHost appHost;

        public UnitTest()
        {
            appHost = new BasicAppHost
            {
                ConfigureAppHost = host =>
                {
                    host.Plugins.Add(new AutoQueryFeature());
                },
                ConfigureContainer = container =>
                {
                    var dbFactory = new OrmLiteConnectionFactory(
                        ":memory:", SqliteDialect.Provider);
                    container.Register<IDbConnectionFactory>(dbFactory);
                    using (var db = dbFactory.Open())
                    {
                        db.DropAndCreateTable<Post>();
                        db.InsertAll(new[] {
                        new Post
                        {
                            Id = 1,
                            MdText = "## First Post",
                            Title = "Test",
                            Path = "test",
                            CreatedBy = "SN",
                            CreatedDate = DateTime.Now,
                            ModifiedBy = "SN",
                            ModifiedDate = DateTime.Now
                        },
                    });
                    }
                    container.RegisterAutoWired<PostsServices>();
                    container.RegisterAutoWired<ILogger<PostsServices>>();
                },
            }.Init();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() => appHost.Dispose();

        // [Test]
        // public async Task Can_call_PostsServices()
        // {
        //     var logger = appHost.Container.Resolve<ILogger<PostsServices>>();
        //     var service = appHost.Container.Resolve<PostsServices>();
        //     service.Request = new BasicRequest();

        //     var response =
        //         (QueryResponse<Post>)(await service.Get(new QueryPosts { Path = "test" }));

        //     Assert.That(response.Results.FirstOrDefault().Path, Is.EqualTo("test"));
        // }
    }
}
