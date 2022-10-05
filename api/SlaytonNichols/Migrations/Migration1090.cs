using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace SlaytonNichols.Migrations
{
    public class Migration1090 : MigrationBase
    {
        public class Post : AuditBase
        {
            [AutoIncrement]
            public int Id { get; set; }
            public string MdText { get; set; }
            public string Title { get; set; }
            public string Path { get; set; }
            public string Summary { get; set; }
        }

        public override void Up()
        {
            Db.CreateTable<Post>();

            CreatePost("## First MD Post!", "Test", "test", "This is a summary");
            CreatePost("## Second MD Post!", "test-two", "test-two", "This is a summary");
        }

        public void CreatePost(string mdText, string title, string path, string summary) =>
            Db.Insert(new Post
            {
                MdText = mdText,
                Title = title,
                Path = path,
                Summary = summary,
                CreatedBy = "Slayton Nichols",
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = "Slayton Nichols",
                ModifiedDate = DateTime.UtcNow,
                // DeletedBy = "Slayton Nichols",
                // DeletedDate = DateTime.UtcNow,
            });

        public override void Down()
        {
            Db.DropTable<Post>();
        }
    }
}
