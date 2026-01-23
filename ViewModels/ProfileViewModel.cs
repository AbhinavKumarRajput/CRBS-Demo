using System.ComponentModel.DataAnnotations;

namespace CRBS.ViewModels
{
    public class ProfileViewModel
    {
      
        public string? Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }

    }
}
