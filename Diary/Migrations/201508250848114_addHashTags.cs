namespace Diary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addHashTags : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HashTags",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.HashTagPosts",
                c => new
                    {
                        HashTag_ID = c.Int(nullable: false),
                        Post_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.HashTag_ID, t.Post_ID })
                .ForeignKey("dbo.HashTags", t => t.HashTag_ID, cascadeDelete: true)
                .ForeignKey("dbo.Posts", t => t.Post_ID, cascadeDelete: true)
                .Index(t => t.HashTag_ID)
                .Index(t => t.Post_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HashTagPosts", "Post_ID", "dbo.Posts");
            DropForeignKey("dbo.HashTagPosts", "HashTag_ID", "dbo.HashTags");
            DropIndex("dbo.HashTagPosts", new[] { "Post_ID" });
            DropIndex("dbo.HashTagPosts", new[] { "HashTag_ID" });
            DropTable("dbo.HashTagPosts");
            DropTable("dbo.HashTags");
        }
    }
}
