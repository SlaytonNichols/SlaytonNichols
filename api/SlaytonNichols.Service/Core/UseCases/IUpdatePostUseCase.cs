using SlaytonNichols.Service.Infrastructure.MongoDb.Entities;
using SlaytonNichols;
using SlaytonNichols.Service.Core.Dtos;

namespace SlaytonNichols.Service.Core.UseCases.UpdatePostUseCase
{
    public interface IUpdatePostUseCase
    {
        Task ExecuteAsync(UpdatePostRequest request);
    }
}