using System;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using rest_api.Context;

namespace rest_api.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<rest_api.Models.MigrationContexts>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        DatabaseContext db = new DatabaseContext();
        protected override void Seed(rest_api.Models.MigrationContexts context)
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

            //Seed Advert Types
            context.AdvertTypes.AddOrUpdate(
                at => at.name,
                new Models.AdvertTypes { name = "Villa"},
                new Models.AdvertTypes { name = "Oda" },
                new Models.AdvertTypes { name = "Tüm Ev" }
                );
            // Seed Cities and Towns
            var sqlfiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "/initialdata", "*.sql");
            sqlfiles.ToList().ForEach(x => context.Database.ExecuteSqlCommand(File.ReadAllText(x)));


        }
    }
}