using System.Data.Entity.Migrations;

namespace MandarinLearner.Model.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<LanguageLearningModel>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "MandarinLearner.Model.LanguageLearningModel";
        }

        protected override void Seed(LanguageLearningModel context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}