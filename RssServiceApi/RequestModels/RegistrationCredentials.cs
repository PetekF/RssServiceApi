using System.ComponentModel.DataAnnotations;

namespace RssServiceApi.RequestModels
{
    public class RegistrationCredentials
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
