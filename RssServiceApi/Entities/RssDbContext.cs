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
    }
}
