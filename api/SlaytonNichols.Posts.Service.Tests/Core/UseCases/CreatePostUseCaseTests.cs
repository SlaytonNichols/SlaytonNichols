using NUnit.Framework;
using Moq;
using AutoFixture;
using Entity = SlaytonNichols.Posts.Service.Infrastructure.Data.Documents.Post;
using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Posts.Service.Core.Mappers;
using System.Threading.Tasks;
using SlaytonNichols.Common.Infrastructure.MongoDb.Repositories;
using SlaytonNichols.Posts.Service.Core.UseCases.CreatePostUseCase;

namespace SlaytonNichols.Posts.Service.Tests.Core.UseCases;

public class CreatePostUseCaseTests
{
    [Test]
    public async Task Success()
    {
        // Arrange
        var fixture = new Fixture();
        var request = fixture.Create<CreatePostRequest>();
        var mapperResponse = fixture.Build<Entity>().With(x => x.Id, new MongoDB.Bson.ObjectId()).Create();

        var mapper = fixture.Freeze<Mock<IPostMapper>>();
        mapper.Setup(c => c.MapCreatePostRequest(request)).Returns(mapperResponse);

        var repo = fixture.Freeze<Mock<IMongoRepository<Entity>>>();
        repo.Setup(c => c.InsertOneAsync(mapperResponse)).Returns(Task.CompletedTask);

        var useCase = new CreatePostUseCase(repo.Object, mapper.Object);

        // Act
        await useCase.ExecuteAsync(request);

        // Assert
        Assert.Null(null);
    }
}