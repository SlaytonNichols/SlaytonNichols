using SlaytonNichols;
using MongoDB.Driver;
using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Posts.Service.Infrastructure.MongoDb.Repositories;
using SlaytonNichols.Posts.Service.Domain.Documents;

namespace SlaytonNichols.Posts.Service.Core.UseCases.GetPostsUseCase
{
    public class GetPostsUseCase : IGetPostsUseCase
    {
        private readonly IMongoRepository<Post> _posts;
        public GetPostsUseCase(IMongoRepository<Post> posts)
        {
            _posts = posts;
        }

        public async Task<IEnumerable<Post>> ExecuteAsync(GetPostsRequest request)
        {
            var posts = await _posts.AsQueryableAsync();
            return posts.Where(x => x.Path == request.Path || string.IsNullOrEmpty(request.Path));
        }
    }
}