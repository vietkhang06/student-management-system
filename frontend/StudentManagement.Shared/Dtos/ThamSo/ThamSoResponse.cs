namespace StudentManagement.Shared.Dtos.ThamSo;

public class ThamSoResponse
{
    public string IdThamSo { get; set; } = string.Empty;
    public string TenThamSo { get; set; } = string.Empty;
    public int KieuThamSo { get; set; }
    public string GiaTriThamSo { get; set; } = string.Empty;
}
