using NUnit.Framework;
using Moq;
using AutoFixture;
using Entity = SlaytonNichols.Posts.Service.Infrastructure.Data.Documents.Post;
using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Posts.Service.Core.Mappers;
using System.Threading.Tasks;
using SlaytonNichols.Common.Infrastructure.MongoDb.Repositories;
using SlaytonNichols.Posts.Service.Core.UseCases.DeletePostUseCase;

namespace SlaytonNichols.Posts.Service.Tests.Core.UseCases;

public class DeletePostUseCaseTests
{
    [Test]
    public async Task Success()
    {
        // Arrange
        var fixture = new Fixture();
        var request = fixture.Create<DeletePostRequest>();

        var repo = fixture.Freeze<Mock<IMongoRepository<Entity>>>();
        repo.Setup(c => c.DeleteByIdAsync(request.Id)).Returns(Task.CompletedTask);

        var useCase = new DeletePostUseCase(repo.Object);

        // Act
        await useCase.ExecuteAsync(request);

        // Assert
        Assert.Null(null);
    }
}