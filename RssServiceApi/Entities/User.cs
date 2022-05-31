using System.ComponentModel.DataAnnotations.Schema;

namespace RssServiceApi.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Token { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? ConfirmedAt { get; set; }

        public ICollection<UserFeed> Subscriptions { get; set; }

        public ICollection<Item> FavoriteItems { get; set; }

        public ICollection<Item> UnreadItems { get; set; }
    }
}
