using System.ComponentModel.DataAnnotations;

namespace RssServiceApi.RequestModels
{
    public class EditUser
    {
        [EmailAddress]
        public string? Email { get; set; }

        [MinLength(6)]
        public string? Password { get; set; }

        [Compare("Password")]
        public string? ConfirmPassword { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
