namespace Core.Models.Authorisation;

public class PasswordUpdateRequest
{
    public string NewPassword { get; set; }
    public string OldPassword { get; set; }
}