using SlaytonNichols.Service.Infrastructure.MongoDb.Entities;
using SlaytonNichols;
using SlaytonNichols.Service.Core.Dtos;

namespace SlaytonNichols.Service.Core.UseCases.CreatePostUseCase
{
    public interface ICreatePostUseCase
    {
        Task ExecuteAsync(CreatePostRequest request);
    }
}