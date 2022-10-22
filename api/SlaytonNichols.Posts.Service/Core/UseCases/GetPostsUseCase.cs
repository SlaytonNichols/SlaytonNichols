using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Common.Infrastructure.MongoDb.Repositories;
using SlaytonNichols.Posts.Service.Infrastructure.Data.Documents;
using SlaytonNichols.Posts.Service.Core.Mappers;

namespace SlaytonNichols.Posts.Service.Core.UseCases.GetPostsUseCase;
public class GetPostsUseCase : IGetPostsUseCase
{
    private readonly IMongoRepository<Post> _posts;
    private readonly IPostMapper _mapper;
    public GetPostsUseCase(IMongoRepository<Post> posts, IPostMapper mapper)
    {
        _posts = posts;
        _mapper = mapper;
    }

    public async Task<GetPostsResponse> ExecuteAsync(GetPostsRequest request)
    {
        var posts = await _posts.AsQueryableAsync();
        var response = new GetPostsResponse
        {
            Results = (await _mapper.Map(posts)).ToList()
        };

        return response;
    }
}
