namespace RssServiceApi.DTOs
{
    public class FeedDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int UnreadItems { get; set; }
        public List<CategoryDto> Categories { get; set; }
        public List<LinkDto> Links { get; set; }
    }
}
