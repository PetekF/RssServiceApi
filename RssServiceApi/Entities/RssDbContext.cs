using EntityFramework.Exceptions.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace RssServiceApi.Entities
{
    public class RssDbContext: DbContext
    {
        public RssDbContext(DbContextOptions<RssDbContext> options)
            : base(options)
        {

        }

        public DbSet<Feed> Feeds { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserFeed> UserFeeds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseExceptionProcessor();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Junction table for favorite items of a user 
            modelBuilder.Entity<User>()
                        .HasMany(u => u.FavoriteItems)
                        .WithMany(i => i.FavoritedBy)
                        .UsingEntity<Dictionary<string, object>>(
                            "FavoriteUserItem",
                            x => x.HasOne<Item>().WithMany().HasForeignKey("ItemsId"),
                            x => x.HasOne<User>().WithMany().HasForeignKey("UsersId"));
                        
            // Junction table for unread items of a user
            modelBuilder.Entity<User>()
                        .HasMany(u => u.UnreadItems)
                        .WithMany(i => i.NotReadBy)
                        .UsingEntity<Dictionary<string, object>>(
                            "UnreadUserItem",
                            x => x.HasOne<Item>().WithMany().HasForeignKey("ItemsId"),
                            x => x.HasOne<User>().WithMany().HasForeignKey("UsersId"));

            // Making a composite key for FeedUser table
            modelBuilder.Entity<UserFeed>()
                        .HasKey(x => new { x.UserId, x.FeedId });
        }
    }
}
