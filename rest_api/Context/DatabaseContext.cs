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
        public DbSet<AdvertImages> advert_images { get; set; }
        public DbSet<AdvertProperties> advert_properties{ get; set; }
        public DbSet<AdvertPossibilities> advert_possibilities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Advert>().ToTable("Advert");
            modelBuilder.Entity<AdvertImages>().ToTable("AdvertImages");
            modelBuilder.Entity<AdvertProperties>().ToTable("AdvertProperties");
            modelBuilder.Entity<AdvertPossibilities>().ToTable("AdvertPossibilities");
        }
    }
}