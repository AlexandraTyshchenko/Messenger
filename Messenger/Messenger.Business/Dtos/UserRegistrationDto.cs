using System.ComponentModel.DataAnnotations;

namespace Messenger.Business.Dtos
{
    public class UserRegistrationDto
    {
        [Required(ErrorMessage = "FirstName is required")]
        public string? FirstName { get; init; }

        [Required(ErrorMessage = "LastName is required")]
        public string? LastName { get; init; }

        [Required(ErrorMessage = "Username is required")]
        public string? UserName { get; init; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; init; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; init; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; init; }
        public string? Bio {  get; init; }
    }
}
