namespace RssServiceApi.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<LinkDto> Links { get; set; }
    }
}
