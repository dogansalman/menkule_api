using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace rest_api.Models
{
    public class MigrationContexts : DbContext
    {
        //ConnectionStrings From Web.Config
        public MigrationContexts() : base(System.Configuration.ConfigurationManager.AppSettings["connection:name"]) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        //Migrations
        public DbSet<Advert> Advert { get; set; }
        public DbSet<AdvertAvailableDate> AdvertAvailableDate { get; set; }
        public DbSet<AdvertComments> AdvertComments { get; set; }
        public DbSet<AdvertFeedbacks> AdvertFeedbacks { get; set; }
        public DbSet<AdvertImages> AdvertImages { get; set; }
        public DbSet<AdvertLikes> AdvertLikes { get; set; }
        public DbSet<AdvertPossibilities> AdvertPossibilities { get; set; }
        public DbSet<AdvertProperties> AdvertProperties { get; set; }
        public DbSet<AdvertScores> AdvertScores { get; set; }
        public DbSet<AdvertTypes> AdvertTypes { get; set; }
        public DbSet<AdvertUnavailableDate> AdvertUnavailableDate { get; set; }
        public DbSet<Cities> Cities { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<RezervationAdverts> RezervationAdverts { get; set; }
        public DbSet<Rezervations> Rezervations { get; set; }
        public DbSet<RezervationVisitors> RezervationVisitors { get; set; }
        public DbSet<Suggetions> Suggetions { get; set; }
        public DbSet<Towns> Towns { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<UserMessages> UserMessages { get; set;}
        public DbSet<Messages> Messages { get; set; }
        public DbSet<AdvertViews> AdvertViews { get; set; }


    }
}