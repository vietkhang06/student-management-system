using System;

namespace StudentManagement.Shared.Dtos.Diem;

public class DiemResponse
{
    public string IdHocSinh { get; set; } = string.Empty;
    public string TenHocSinh { get; set; } = string.Empty;
    public string IdMonHoc { get; set; } = string.Empty;
    public string IdHocKy { get; set; } = string.Empty;
    public decimal? Diem15 { get; set; }
    public decimal? Diem45 { get; set; }
    public decimal? DiemCk { get; set; }
    public decimal? DiemTb { get; set; }
}
