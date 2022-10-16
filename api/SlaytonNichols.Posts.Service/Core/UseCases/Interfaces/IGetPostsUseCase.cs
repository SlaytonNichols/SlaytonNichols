using SlaytonNichols;
using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Posts.Service.Domain.Documents;

namespace SlaytonNichols.Posts.Service.Core.UseCases.GetPostsUseCase
{
    public interface IGetPostsUseCase
    {
        Task<IEnumerable<Post>> ExecuteAsync(GetPostsRequest request);
    }
}