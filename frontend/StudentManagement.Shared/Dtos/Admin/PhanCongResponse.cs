namespace StudentManagement.Shared.Dtos.Admin;

public sealed class PhanCongResponse
{
    public string IdPhanCong { get; set; } = string.Empty;
    public string IdGiaoVien { get; set; } = string.Empty;
    public string TenGiaoVien { get; set; } = string.Empty;
    public string IdLop { get; set; } = string.Empty;
    public string TenLop { get; set; } = string.Empty;
    public string IdMonHoc { get; set; } = string.Empty;
    public string TenMonHoc { get; set; } = string.Empty;
    public string IdHocKy { get; set; } = string.Empty;
    public string TenHocKy { get; set; } = string.Empty;
}
