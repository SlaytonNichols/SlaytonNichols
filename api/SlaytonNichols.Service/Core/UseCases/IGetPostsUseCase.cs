using SlaytonNichols.Service.Infrastructure.MongoDb.Entities;
using SlaytonNichols;
using SlaytonNichols.Service.Core.Dtos;

namespace SlaytonNichols.Service.Core.UseCases.GetPostsUseCase
{
    public interface IGetPostsUseCase
    {
        Task<IEnumerable<Post>> ExecuteAsync(GetPostsRequest request);
    }
}