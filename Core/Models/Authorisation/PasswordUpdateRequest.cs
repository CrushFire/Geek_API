using System.ComponentModel.DataAnnotations;

namespace Core.Models.Authorisation;

public class PasswordUpdateRequest
{
    public string Password { get; set; }
}
