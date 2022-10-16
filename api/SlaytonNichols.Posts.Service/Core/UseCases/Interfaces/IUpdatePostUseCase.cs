using SlaytonNichols;
using SlaytonNichols.Posts.Service.Core.Dtos;

namespace SlaytonNichols.Posts.Service.Core.UseCases.UpdatePostUseCase
{
    public interface IUpdatePostUseCase
    {
        Task ExecuteAsync(UpdatePostRequest request);
    }
}