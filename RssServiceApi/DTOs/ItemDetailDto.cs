namespace RssServiceApi.DTOs
{
    public class ItemDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        // This is link to some external webpage, not a link internal to this app
        public string Link { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsRead { get; set; }
        public List<CategoryDto> Categories { get; set; }
        public List<LinkDto> Links { get; set; }
    }
}
