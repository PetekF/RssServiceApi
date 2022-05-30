using Microsoft.EntityFrameworkCore;
using RssServiceApi.Entities;
namespace RssServiceApi.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public List<LinkDto> Links { get; set; }
    }
}
