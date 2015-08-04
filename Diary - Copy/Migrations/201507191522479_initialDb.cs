namespace Diary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BlogPosts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PostDate = c.DateTime(nullable: false),
                        Heading = c.String(nullable: false, maxLength: 100),
                        Post = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TagName = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TagBlogPosts",
                c => new
                    {
                        Tag_ID = c.Int(nullable: false),
                        BlogPost_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_ID, t.BlogPost_ID })
                .ForeignKey("dbo.Tags", t => t.Tag_ID, cascadeDelete: true)
                .ForeignKey("dbo.BlogPosts", t => t.BlogPost_ID, cascadeDelete: true)
                .Index(t => t.Tag_ID)
                .Index(t => t.BlogPost_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagBlogPosts", "BlogPost_ID", "dbo.BlogPosts");
            DropForeignKey("dbo.TagBlogPosts", "Tag_ID", "dbo.Tags");
            DropIndex("dbo.TagBlogPosts", new[] { "BlogPost_ID" });
            DropIndex("dbo.TagBlogPosts", new[] { "Tag_ID" });
            DropTable("dbo.TagBlogPosts");
            DropTable("dbo.Tags");
            DropTable("dbo.BlogPosts");
        }
    }
}
