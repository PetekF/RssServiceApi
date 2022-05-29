namespace RssServiceApi.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Slug { get; set; }
        public Category? Parent { get; set; }

        public User Creator { get; set; }

        public ICollection<Item> Items { get; set; }
        public ICollection<Feed> Feeds { get; set; }
    }
}
