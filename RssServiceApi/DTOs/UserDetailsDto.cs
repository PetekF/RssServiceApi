namespace RssServiceApi.DTOs
{
    public class UserDetailsDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<LinkDto> Links { get; set; }
    }
}
