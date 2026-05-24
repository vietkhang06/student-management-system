namespace StudentManagement.Shared.Dtos.Auth;

public sealed class AuthResponse
{
    public string? Message { get; set; }
    public string? IdTaiKhoan { get; set; }
    public string? TenDangNhap { get; set; }
    public string? LoaiTaiKhoan { get; set; }
    public string? Token { get; set; }
}
