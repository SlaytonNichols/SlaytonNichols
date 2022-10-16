using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Posts.Service.Domain.Documents;
using SlaytonNichols.Common.Infrastructure.MongoDb.Repositories;

namespace SlaytonNichols.Posts.Service.Core.UseCases.UpdatePostUseCase
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