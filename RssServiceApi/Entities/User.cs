using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RssServiceApi.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string EmailConfirmationKey { get; set; }

        [DefaultValue(false)]
        public bool EmailConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<UserFeed> Subscriptions { get; set; }

        public ICollection<Item> FavoriteItems { get; set; }

        public ICollection<Item> UnreadItems { get; set; }
    }
}
