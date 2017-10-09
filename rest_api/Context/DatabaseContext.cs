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
        public DbSet<AdvertTypes> advert_types { get; set; }
        public DbSet<AdvertUnavailableDate>advert_unavaiable_dates { get; set; }
        public DbSet<AdvertAvailableDate>advert_avaiable_dates { get; set; }
        public DbSet<Images>images { get; set; }
        public DbSet<AdvertScores> advert_scores { get; set; }
        public DbSet<AdvertFeedbacks> advert_feedbakcs { get; set; }
        public DbSet<Cities> cities { get; set; }
        public DbSet<Towns> towns { get; set; }
        public DbSet<AdvertComments> advert_comments { get; set; }
        public DbSet<Users> users { get; set; }
        public DbSet<Suggetions> suggestions { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Advert>().ToTable("Advert");
            modelBuilder.Entity<AdvertImages>().ToTable("AdvertImages");
            modelBuilder.Entity<AdvertProperties>().ToTable("AdvertProperties");
            modelBuilder.Entity<AdvertPossibilities>().ToTable("AdvertPossibilities");
            modelBuilder.Entity<AdvertTypes>().ToTable("AdvertTypes");
            modelBuilder.Entity<AdvertUnavailableDate>().ToTable("AdvertUnavailableDate");
            modelBuilder.Entity<AdvertAvailableDate>().ToTable("AdvertAvailableDate");
            modelBuilder.Entity<Images>().ToTable("Images");
            modelBuilder.Entity<AdvertScores>().ToTable("AdvertScores");
            modelBuilder.Entity<AdvertFeedbacks>().ToTable("AdvertFeedbacks");
            modelBuilder.Entity<Cities>().ToTable("Cities");
            modelBuilder.Entity<Towns>().ToTable("Towns");
            modelBuilder.Entity<AdvertComments>().ToTable("AdvertComments");
            modelBuilder.Entity<Users>().ToTable("users");
            modelBuilder.Entity<Suggetions>().ToTable("Suggetions");



        }
    }
}