namespace StudentManagement.Shared.Dtos.BaoCao;

public class BaoCaoMonResponse
{
    public string IdLop { get; set; } = string.Empty;
    public string TenLop { get; set; } = string.Empty;
    public string IdMonHoc { get; set; } = string.Empty;
    public string TenMonHoc { get; set; } = string.Empty;
    public string IdHocKy { get; set; } = string.Empty;
    public int TongHocSinh { get; set; }
    public int SoLuongDat { get; set; }
    public double TyLeDat { get; set; }
}
