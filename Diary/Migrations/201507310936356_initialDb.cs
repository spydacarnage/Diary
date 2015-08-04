namespace Diary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PostDate = c.DateTime(nullable: false),
                        Heading = c.String(maxLength: 100),
                        Body = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.PostCategories",
                c => new
                    {
                        Post_ID = c.Int(nullable: false),
                        Category_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Post_ID, t.Category_ID })
                .ForeignKey("dbo.Posts", t => t.Post_ID, cascadeDelete: true)
                .ForeignKey("dbo.Categories", t => t.Category_ID, cascadeDelete: true)
                .Index(t => t.Post_ID)
                .Index(t => t.Category_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PostCategories", "Category_ID", "dbo.Categories");
            DropForeignKey("dbo.PostCategories", "Post_ID", "dbo.Posts");
            DropIndex("dbo.PostCategories", new[] { "Category_ID" });
            DropIndex("dbo.PostCategories", new[] { "Post_ID" });
            DropTable("dbo.PostCategories");
            DropTable("dbo.Posts");
            DropTable("dbo.Categories");
        }
    }
}
