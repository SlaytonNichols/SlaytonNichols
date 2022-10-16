using SlaytonNichols.Posts.Service.Core.UseCases.CreatePostUseCase;
using SlaytonNichols.Posts.Service.Core.UseCases.DeletePostUseCase;
using SlaytonNichols.Posts.Service.Core.UseCases.GetPostsUseCase;
using SlaytonNichols.Posts.Service.Core.UseCases.UpdatePostUseCase;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using SlaytonNichols.Posts.Service.Domain.Documents;
using SlaytonNichols.Common.Infrastructure.MongoDb.Repositories;

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
    }
}