using SlaytonNichols.Posts.Service.Core.Dtos;

namespace SlaytonNichols.Posts.Service.Core.UseCases.CreatePostUseCase;
public interface ICreatePostUseCase
{
    Task ExecuteAsync(CreatePostRequest request);
}
