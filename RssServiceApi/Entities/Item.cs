using System.ComponentModel.DataAnnotations.Schema;

namespace RssServiceApi.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public Feed Feed { get; set; }
        public string Title { get; set; }
        public string? Link { get; set; }
        public string? Guid { get; set; }
        public string Description { get; set; }
        public DateTime? PubDate { get; set; }
        public string Hash { get; set; }

        public ICollection<Category> Categories { get; set; }

        public ICollection<User> FavoritedBy { get; set; }

        public ICollection<User> NotReadBy { get; set; }
    }
}
