using SlaytonNichols.Service.Infrastructure.MongoDb.Entities;
using SlaytonNichols;
using MongoDB.Driver;
using SlaytonNichols.Service.Core.Dtos;
using MongoDB.Bson;
using SlaytonNichols.Service.Infrastructure.MongoDb.Repositories;

namespace SlaytonNichols.Service.Core.UseCases.DeletePostUseCase
{
    public class DeletePostUseCase : IDeletePostUseCase
    {
        private readonly IMongoRepository<Post> _posts;
        public DeletePostUseCase(IMongoRepository<Post> posts)
        {
            _posts = posts;
        }

        public async Task ExecuteAsync(DeletePostRequest request)
        {
            await _posts.DeleteOneAsync(x => x.Id == request.Id);
        }

    }
}