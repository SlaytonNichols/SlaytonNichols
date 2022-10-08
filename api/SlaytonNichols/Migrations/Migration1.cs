using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace SlaytonNichols.Migrations
{
    public class Migration1 : MigrationBase
    {
        public override void Up()
        {
            Db.ExecuteSql(@"
IF NOT EXISTS(SELECT 1 FROM sys.tables t where t.name = 'Post')
BEGIN
	CREATE TABLE [dbo].[Post](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[MdText] [varchar](8000) NULL,
		[Title] [varchar](8000) NULL,
		[Path] [varchar](8000) NULL,
		[Summary] [varchar](8000) NULL,
		[CreatedDate] [datetime] NOT NULL,
		[CreatedBy] [varchar](8000) NOT NULL,
		[ModifiedDate] [datetime] NOT NULL,
		[ModifiedBy] [varchar](8000) NOT NULL,
		[DeletedDate] [datetime] NULL,
		[DeletedBy] [varchar](8000) NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
            ");
            Db.ExecuteSql(@"
IF NOT EXISTS(SELECT 1 FROM dbo.Post WHERE Id = 1)
BEGIN
    INSERT INTO [dbo].[Post] ([MdText], [Title], [Path], [Summary], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [DeletedDate], [DeletedBy])
    VALUES (N'## This is a test post', N'Test Post', N'/test-post', N'This is a test post', N'2015-01-01 00:00:00', N'admin', N'2015-01-01 00:00:00', N'admin', N'2015-01-01 00:00:00', N'admin')
END
            ");

            Db.ExecuteSql(@"
IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Draft'
          AND Object_ID = Object_ID(N'dbo.Post'))
BEGIN
	ALTER TABLE [dbo].[Post] ADD Draft BIT CONSTRAINT DF_Post_Draft DEFAULT ((0)) NOT NULL
END
            ");
        }
    }
}
