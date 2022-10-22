using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Posts.Service.Domain.Documents;
using SlaytonNichols.Common.Infrastructure.MongoDb.Repositories;

namespace SlaytonNichols.Posts.Service.Core.UseCases.GetPostsUseCase;
public class GetPostsUseCase : IGetPostsUseCase
{
    private readonly IMongoRepository<Post> _posts;
    public GetPostsUseCase(IMongoRepository<Post> posts)
    {
        _posts = posts;
    }

    public async Task<GetPostsResponse> ExecuteAsync(GetPostsRequest request)
    {
        var posts = await _posts.AsQueryableAsync();
        var response = new GetPostsResponse
        {
            Results = posts.ToList()
        };

        return response;
    }
}
