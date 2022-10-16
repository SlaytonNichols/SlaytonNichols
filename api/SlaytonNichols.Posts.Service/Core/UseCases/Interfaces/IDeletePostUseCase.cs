
using SlaytonNichols;
using SlaytonNichols.Posts.Service.Core.Dtos;

namespace SlaytonNichols.Posts.Service.Core.UseCases.DeletePostUseCase
{
    public interface IDeletePostUseCase
    {
        Task ExecuteAsync(DeletePostRequest request);
    }
}