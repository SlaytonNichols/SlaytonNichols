
using MongoDB.Driver;
using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Posts.Service.Domain.Documents;
using SlaytonNichols.Common.Infrastructure.MongoDb.Repositories;

namespace SlaytonNichols.Posts.Service.Core.UseCases.CreatePostUseCase;

public class CreatePostUseCase : ICreatePostUseCase
{
    private readonly IMongoRepository<Post> _posts;
    public CreatePostUseCase(IMongoRepository<Post> posts)
    {
        _posts = posts;
    }

    public async Task ExecuteAsync(CreatePostRequest request)
    {
        var post = new Post
        {
            MdText = request.MdText,
            Title = request.Title,
            Path = request.Path,
            Summary = request.Summary,
            Draft = request.Draft
        };

        await _posts.InsertOneAsync(post);
    }
}
