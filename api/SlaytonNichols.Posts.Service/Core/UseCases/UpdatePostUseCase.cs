using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Common.Infrastructure.MongoDb.Repositories;
using SlaytonNichols.Posts.Service.Core.Mappers;
using SlaytonNichols.Posts.Service.Infrastructure.Data.Documents;

namespace SlaytonNichols.Posts.Service.Core.UseCases.UpdatePostUseCase;
public class UpdatePostUseCase : IUpdatePostUseCase
{
    private readonly IMongoRepository<Post> _posts;
    private readonly IPostMapper _mapper;
    public UpdatePostUseCase(IMongoRepository<Post> posts, IPostMapper mapper)
    {
        _posts = posts;
        _mapper = mapper;
    }

    public async Task ExecuteAsync(UpdatePostRequest request)
    {
        var mapped = await _mapper.MapUpdatePostRequest(request);
        await _posts.ReplaceOneAsync(mapped);
    }
}
