using SlaytonNichols.Service.Infrastructure.MongoDb.Entities;
using SlaytonNichols;
using MongoDB.Driver;
using SlaytonNichols.Service.Core.Dtos;
using MongoDB.Bson;
using SlaytonNichols.Service.Infrastructure.MongoDb.Repositories;

namespace SlaytonNichols.Service.Core.UseCases.UpdatePostUseCase
{
    public class UpdatePostUseCase : IUpdatePostUseCase
    {
        private readonly IMongoRepository<Post> _posts;
        public UpdatePostUseCase(IMongoRepository<Post> posts)
        {
            _posts = posts;
        }

        public async Task ExecuteAsync(UpdatePostRequest request)
        {
            await _posts.ReplaceOneAsync(request);
        }

    }
}