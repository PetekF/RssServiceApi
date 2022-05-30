namespace RssServiceApi.DTOs
{
    public class FeedDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<LinkDto> Links { get; set; }
    }
}
