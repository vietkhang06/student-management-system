namespace StudentManagement.Shared.Dtos.BaoCao;

public class BaoCaoHocKyResponse
{
    public string IdLop { get; set; } = string.Empty;
    public string TenLop { get; set; } = string.Empty;
    public string IdHocKy { get; set; } = string.Empty;
    public int TongHocSinh { get; set; }
    public int SoLuongDat { get; set; }
    public double TyLeDat { get; set; }
}
