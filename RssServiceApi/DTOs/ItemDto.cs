namespace RssServiceApi.DTOs
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsRead { get; set; }
        public List<LinkDto> Links { get; set; }
    }
}
