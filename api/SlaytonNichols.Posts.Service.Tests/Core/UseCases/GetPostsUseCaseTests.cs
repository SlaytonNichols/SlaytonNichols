using NUnit.Framework;
using Moq;
using AutoFixture;
using Entity = SlaytonNichols.Posts.Service.Infrastructure.Data.Documents.Post;
using SlaytonNichols.Posts.Service.Domain.Models;
using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Posts.Service.Core.Mappers;
using System.Threading.Tasks;
using SlaytonNichols.Common.Infrastructure.MongoDb.Repositories;
using System.Linq;
using SlaytonNichols.Posts.Service.Core.UseCases.GetPostsUseCase;

namespace SlaytonNichols.Posts.Service.Tests.Core.UseCases;

public class GetPostsUseCaseTests
{
    [Test]
    public async Task Success()
    {
        // Arrange
        var fixture = new Fixture();
        var request = fixture.Create<GetPostsRequest>();
        var repoResponse = fixture.Build<Entity>().With(x => x.Id, new MongoDB.Bson.ObjectId()).CreateMany().AsQueryable();
        var mapperResponse = fixture.CreateMany<Post>();

        var repo = fixture.Freeze<Mock<IMongoRepository<Entity>>>();
        repo.Setup(c => c.AsQueryableAsync()).Returns(Task.FromResult(repoResponse));

        var mapper = fixture.Freeze<Mock<IPostMapper>>();
        mapper.Setup(c => c.Map(repoResponse.ToList())).Returns(mapperResponse);

        var useCase = new GetPostsUseCase(repo.Object, mapper.Object);

        // Act
        var result = await useCase.ExecuteAsync(request);

        // Assert
        Assert.NotNull(result);
    }
}