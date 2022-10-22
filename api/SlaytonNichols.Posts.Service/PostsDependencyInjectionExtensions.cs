using SlaytonNichols.Posts.Service.Core.UseCases.CreatePostUseCase;
using SlaytonNichols.Posts.Service.Core.UseCases.DeletePostUseCase;
using SlaytonNichols.Posts.Service.Core.UseCases.GetPostsUseCase;
using SlaytonNichols.Posts.Service.Core.UseCases.UpdatePostUseCase;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using SlaytonNichols.Common.Infrastructure.MongoDb.Repositories;
using SlaytonNichols.Posts.Service.Infrastructure.Data.Documents;
using SlaytonNichols.Posts.Service.Core.Mappers;

namespace SlaytonNichols.Posts.Service;

public static class PostsDependencyInjectionExtensions
{
    public static void AddPostsService(this IServiceCollection services)
    {
        BsonClassMap.RegisterClassMap<Post>();
        services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        services.AddSingleton<IGetPostsUseCase, GetPostsUseCase>();
        services.AddSingleton<ICreatePostUseCase, CreatePostUseCase>();
        services.AddSingleton<IUpdatePostUseCase, UpdatePostUseCase>();
        services.AddSingleton<IDeletePostUseCase, DeletePostUseCase>();
        services.AddSingleton<IPostMapper, PostMapper>();
    }
}