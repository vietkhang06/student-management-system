namespace StudentManagement.Shared.Dtos.HocSinh;

public class HocSinhSummaryResponse
{
    public string IdHocSinh { get; set; } = string.Empty;
    public string Ten { get; set; } = string.Empty;
    public string IdLop { get; set; } = string.Empty;
    public double? TbHocKy1 { get; set; }
    public double? TbHocKy2 { get; set; }
}
