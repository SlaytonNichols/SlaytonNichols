using SlaytonNichols.Service.Infrastructure.MongoDb.Entities;
using SlaytonNichols;
using SlaytonNichols.Service.Core.Dtos;

namespace SlaytonNichols.Service.Core.UseCases.DeletePostUseCase
{
    public interface IDeletePostUseCase
    {
        Task ExecuteAsync(DeletePostRequest request);
    }
}