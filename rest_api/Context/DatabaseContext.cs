using System.Data.Entity;
using rest_api.Models;

namespace rest_api.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("Name=ConnectionStr")
        {
            Database.SetInitializer<DatabaseContext>(null);

        }
        public DbSet<Advert> advert { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Advert>().ToTable("adverts");
        }
    }
}