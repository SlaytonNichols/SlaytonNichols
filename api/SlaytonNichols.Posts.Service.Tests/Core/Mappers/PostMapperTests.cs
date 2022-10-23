using AutoFixture;
using NUnit.Framework;
using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Posts.Service.Core.Mappers;
using Entity = SlaytonNichols.Posts.Service.Infrastructure.Data.Documents.Post;

namespace SlaytonNichols.Posts.Service.Tests.Core.Mappers;

public class PostMapperTests
{
    [Test]
    public void MapCreatePostRequest()
    {
        // Arrange
        var fixture = new Fixture();
        var request = fixture.Create<CreatePostRequest>();
        var mapperResponse = fixture.Build<Entity>().With(x => x.Id, new MongoDB.Bson.ObjectId()).Create();

        var mapper = new PostMapper();

        // Act
        var result = mapper.MapCreatePostRequest(request);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(request.Title, result.Title);
        Assert.AreEqual(request.MdText, result.MdText);
        Assert.AreEqual(request.Draft, result.Draft);
        Assert.AreEqual(request.Path, result.Path);
        Assert.AreEqual(request.Summary, result.Summary);
    }

    [Test]
    public void MapUpdatePostRequest()
    {
        // Arrange
        var fixture = new Fixture();
        var request = fixture.Build<UpdatePostRequest>().With(x => x.Id, "635081516c2ef0b6f6a5b7e9").Create();
        var objId = new MongoDB.Bson.ObjectId(request.Id);
        var mapperResponse = fixture.Build<Entity>().With(x => x.Id, objId).Create();

        var mapper = new PostMapper();

        // Act
        var result = mapper.MapUpdatePostRequest(request);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(request.Title, result.Title);
        Assert.AreEqual(request.MdText, result.MdText);
        Assert.AreEqual(request.Draft, result.Draft);
        Assert.AreEqual(request.Path, result.Path);
        Assert.AreEqual(request.Summary, result.Summary);
        Assert.AreEqual(objId, result.Id);
    }
}