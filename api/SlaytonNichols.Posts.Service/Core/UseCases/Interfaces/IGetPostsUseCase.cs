using SlaytonNichols.Posts.Service.Core.Dtos;

namespace SlaytonNichols.Posts.Service.Core.UseCases.GetPostsUseCase;
public interface IGetPostsUseCase
{
    Task<GetPostsResponse> ExecuteAsync(GetPostsRequest request);
}
