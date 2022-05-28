namespace RssServiceApi.Entities
{
    public class Feed
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string? Descciption { get; set; }
        public DateTime? LastBuildDate { get; set; }
        public DateTime LastModified { get; set; }
    }
}
