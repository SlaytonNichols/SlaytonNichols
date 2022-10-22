using MongoDB.Bson.Serialization.Attributes;
using SlaytonNichols.Common.Infrastructure.MongoDb;

namespace SlaytonNichols.Posts.Service.Domain.Models;
public class Post
{
    public string Id { get; set; }
    public string MdText { get; set; }
    public string Title { get; set; }
    public string Path { get; set; }
    public string Summary { get; set; }
    public bool Draft { get; set; }
}
