namespace StudentManagement.Shared.Dtos.Admin;

public sealed class PhanCongRequest
{
    public string IdGiaoVien { get; set; } = string.Empty;
    public string IdLop { get; set; } = string.Empty;
    public string IdMonHoc { get; set; } = string.Empty;
    public string IdHocKy { get; set; } = string.Empty;
}
