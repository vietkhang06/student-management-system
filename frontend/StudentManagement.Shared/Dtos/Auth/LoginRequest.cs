namespace StudentManagement.Shared.Dtos.Auth;

public sealed class LoginRequest
{
    public string TenDangNhap { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
}
