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

            //Seed Advert Types
            context.AdvertTypes.AddOrUpdate(
                at => at.name,
                new Models.AdvertTypes { name = "Villa"},
                new Models.AdvertTypes { name = "Oda" },
                new Models.AdvertTypes { name = "Tüm Ev" }
                );

            // Seed Cities and Towns
            var sqlfiles = Directory.GetFiles(AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + "/initialdata", "*.sql");
            sqlfiles.ToList().ForEach(x => context.Database.ExecuteSqlCommand(File.ReadAllText(x)));
        }
    }
}