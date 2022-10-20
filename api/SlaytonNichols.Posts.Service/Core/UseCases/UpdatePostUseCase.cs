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
            var post = new Post
            {
                Id = request.Id,
                MdText = request.MdText,
                Title = request.Title,
                Path = request.Path,
                Summary = request.Summary,
                Draft = request.Draft
            };
            await _posts.ReplaceOneAsync(request);
        }

    }
}