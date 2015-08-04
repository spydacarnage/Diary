namespace Diary.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Diary.Models.BlogContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BlogContext context)
        {
            context.Categories.AddOrUpdate(
                c => c.Name,
                new Category { Name = "Mark" },
                new Category { Name = "Asimov" },
                new Category { Name = "Cassius" },
                new Category { Name = "Damien" },
                new Category { Name = "Event" },
                new Category { Name = "Birthday" }
                );
        }
    }
}
