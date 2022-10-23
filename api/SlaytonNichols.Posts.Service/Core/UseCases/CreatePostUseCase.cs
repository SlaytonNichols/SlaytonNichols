
using MongoDB.Driver;
using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Common.Infrastructure.MongoDb.Repositories;
using SlaytonNichols.Posts.Service.Infrastructure.Data.Documents;
using SlaytonNichols.Posts.Service.Core.Mappers;

namespace SlaytonNichols.Posts.Service.Core.UseCases.CreatePostUseCase;

public class CreatePostUseCase : ICreatePostUseCase
{
    private readonly IMongoRepository<Post> _posts;
    private readonly IPostMapper _mapper;
    public CreatePostUseCase(IMongoRepository<Post> posts, IPostMapper mapper)
    {
        _posts = posts;
        _mapper = mapper;
    }

    public async Task ExecuteAsync(CreatePostRequest request)
    {
        var mapped = _mapper.MapCreatePostRequest(request);

        await _posts.InsertOneAsync(mapped);
    }
}
