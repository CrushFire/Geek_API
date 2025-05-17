using System.ComponentModel.DataAnnotations;

namespace Core.Models.Authorisation
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "RequiredUserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "RequiredEmail")]
        [EmailAddress(ErrorMessage = "InvalidEmailFormat")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "RequiredPassword")]
        [MinLength(8, ErrorMessage = "MinLengthPassword")]
        public string Password { get; set; }
    }
}