using System.ComponentModel.DataAnnotations.Schema;

namespace RssServiceApi.Entities
{
    // This class shows feeds to which users are subscribed. It is created as a 
    // separate entity because of additional information about this relationship,
    // e.g. a name that user gives to this feed
    public class UserFeed
    {
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public int FeedId { get; set; }

        [ForeignKey("FeedId")]
        public Feed Feed { get; set; }

        public string? FeedName { get; set; }
    }
}
