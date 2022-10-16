using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SlaytonNichols.Service.Infrastructure.MongoDb.Entities
{
    [BsonCollectionAttribute("Posts")]
    public class Post : IDocument
    {

        public string MdText { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Summary { get; set; }
        public bool Draft { get; set; }
        public ObjectId Id { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}