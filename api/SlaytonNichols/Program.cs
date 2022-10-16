using SlaytonNichols.Service.Core.UseCases.CreatePostUseCase;
using SlaytonNichols.Service.Core.UseCases.DeletePostUseCase;
using SlaytonNichols.Service.Core.UseCases.GetPostsUseCase;
using SlaytonNichols.Service.Core.UseCases.UpdatePostUseCase;
using SlaytonNichols.Service.Infrastructure.MongoDb.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
builder.Services.AddSingleton<IGetPostsUseCase, GetPostsUseCase>();
builder.Services.AddSingleton<ICreatePostUseCase, CreatePostUseCase>();
builder.Services.AddSingleton<IUpdatePostUseCase, UpdatePostUseCase>();
builder.Services.AddSingleton<IDeletePostUseCase, DeletePostUseCase>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseServiceStack(new AppHost());

app.Run();
