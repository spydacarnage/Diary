namespace Diary.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Diary.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Diary.Models.DB>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Diary.Models.DB context)
        {
            context.Tags.AddOrUpdate(
                t => t.TagName,
                new Tag { TagName = "Asimov" },
                new Tag { TagName = "Damien" },
                new Tag { TagName = "Cassius" },
                new Tag { TagName = "Mark" }
            );

            context.BlogPosts.AddOrUpdate(
                b => b.Heading,
                new BlogPost
                {
                    Heading = "London game",
                    PostDate = new DateTime(2015, 7, 4),
                    Tags = new List<Tag> { context.Tags.Find(1) },
                    Post = "There was a game!!"
                }
            );
        }
    }
}
