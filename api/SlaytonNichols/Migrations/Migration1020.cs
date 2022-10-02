using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace SlaytonNichols.Migrations
{
    public class Migration1020 : MigrationBase
    {
        public class Post : AuditBase
        {
            [AutoIncrement]
            public int Id { get; set; }
            public string MdText { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
        }

        public override void Up()
        {
            Db.CreateTable<Post>();

            CreatePost("## First MD Post!", "Test", "test");
            CreatePost("## Second MD Post!", "test-two", "test-two");
        }

        public void CreatePost(string mdText, string name, string path) =>
            Db.Insert(new Post
            {
                MdText = mdText,
                Name = name,
                Path = path,
                CreatedBy = "Slayton Nichols",
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = "Slayton Nichols",
                ModifiedDate = DateTime.UtcNow,
            });

        public override void Down()
        {
            Db.DropTable<Post>();
        }
    }
}
