using System.ComponentModel.DataAnnotations;

namespace RssServiceApi.RequestModels
{
    public class RegistrationCredentials
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
