namespace StudentManagement.Shared.Dtos.Diem;

public class DiemCreateRequest
{
    public string IdHocSinh { get; set; } = string.Empty;
    public string IdMonHoc { get; set; } = string.Empty;
    public string IdHocKy { get; set; } = string.Empty;
    public decimal? Diem15 { get; set; }
    public decimal? Diem45 { get; set; }
    public decimal? DiemCk { get; set; }
}
