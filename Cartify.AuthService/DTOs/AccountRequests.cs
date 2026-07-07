namespace Cartify.AuthService.DTOs;

public class UpdateProfileRequest
{
    public string Email { get; set; } = "";
    public string FullName { get; set; } = "";
}

public class ChangePasswordRequest
{
    public string Email { get; set; } = "";
    public string CurrentPassword { get; set; } = "";
    public string NewPassword { get; set; } = "";
}

public class VerifyOtpRequest
{
    public string Email { get; set; } = "";
    public string Code { get; set; } = "";
}
