using SlaytonNichols;
using MongoDB.Driver;
using SlaytonNichols.Posts.Service.Core.Dtos;
using MongoDB.Bson;
using SlaytonNichols.Posts.Service.Infrastructure.MongoDb.Repositories;
using SlaytonNichols.Posts.Service.Domain.Documents;

namespace SlaytonNichols.Posts.Service.Core.UseCases.DeletePostUseCase
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