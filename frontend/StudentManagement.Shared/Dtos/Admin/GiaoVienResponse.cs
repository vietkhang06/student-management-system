namespace StudentManagement.Shared.Dtos.Admin;

public sealed class GiaoVienResponse
{
    public string IdGiaoVien { get; set; } = string.Empty;
    public string TenGiaoVien { get; set; } = string.Empty;
    public string TenDangNhap { get; set; } = string.Empty;
    public string Sdt { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string GioiTinh { get; set; } = string.Empty;
    public string IdLop { get; set; } = string.Empty;
    public string TenLop { get; set; } = string.Empty;
    public string IdLopChuNhiem { get; set; } = string.Empty;
    public string TenLopChuNhiem { get; set; } = string.Empty;
    public string IdMonHoc { get; set; } = string.Empty;
    public string TenMonHoc { get; set; } = string.Empty;
    public bool Active { get; set; } = true;
}
