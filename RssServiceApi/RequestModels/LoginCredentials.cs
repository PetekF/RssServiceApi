using System.ComponentModel.DataAnnotations;

namespace RssServiceApi.RequestModels
{
    public class LoginCredentials
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
