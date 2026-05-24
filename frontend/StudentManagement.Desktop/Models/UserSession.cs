namespace StudentManagement.Desktop.Models;

public sealed class UserSession
{
    public string IdTaiKhoan { get; init; } = string.Empty;
    public string TenDangNhap { get; init; } = string.Empty;
    public string LoaiTaiKhoan { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;

    public bool IsBanQuanLy => string.Equals(LoaiTaiKhoan, UserRoles.BanQuanLy, StringComparison.OrdinalIgnoreCase);
    public bool IsGiaoVien => string.Equals(LoaiTaiKhoan, UserRoles.GiaoVien, StringComparison.OrdinalIgnoreCase);
}
