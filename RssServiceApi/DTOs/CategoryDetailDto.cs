namespace RssServiceApi.DTOs
{
    public class CategoryDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public CategoryDto Parent { get; set; }
        public List<LinkDto> Links { get; set; }
    }
}
