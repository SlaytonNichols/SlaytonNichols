using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SlaytonNichols.Common.Infrastructure.MongoDb;
using System;

namespace SlaytonNichols.Posts.Service.Domain.Documents;

[BsonCollectionAttribute("Posts")]
[BsonDiscriminator("Post")]
public class Post : Document
{
    public string MdText { get; set; }
    public string Title { get; set; }
    public string Path { get; set; }
    public string Summary { get; set; }
    public bool Draft { get; set; }
}
