using System;
using System.Linq;
using System.Threading.Tasks;
using Funq;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using SlaytonNichols.ServiceInterface;
using SlaytonNichols.ServiceModel;

namespace SlaytonNichols.Tests
{
    public class IntegrationTest
    {
        const string BaseUri = "http://localhost:2000/";

        private readonly ServiceStackHost appHost;

        class AppHost : AppSelfHostBase
        {
            public AppHost() :
                base(nameof(IntegrationTest), typeof(PostsServices).Assembly)
            {
                Plugins.Add(new AutoQueryFeature());
            }

            public override void Configure(Container container)
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
                            Name = "Test",
                            Path = "/posts/test",
                            CreatedBy = "SN",
                            CreatedDate = DateTime.Now,
                            ModifiedBy = "SN",
                            ModifiedDate = DateTime.Now
                        }
                    });
                }
                container.RegisterAutoWired<PostsServices>();
            }
        }

        public IntegrationTest()
        {
            appHost = new AppHost().Init().Start(BaseUri);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() => appHost.Dispose();

        public IServiceClient CreateClient() => new JsonServiceClient(BaseUri);

        [Test]
        public void Can_call_Posts_Service()
        {
            var client = CreateClient();

            var response = client.Get(new QueryPosts { Path = "test" });

            Assert.That(response.Results.First().Id, Is.EqualTo(1));
        }
    }
}
