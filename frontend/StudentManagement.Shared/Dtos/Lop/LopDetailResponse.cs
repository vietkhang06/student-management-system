using System.Collections.Generic;
using StudentManagement.Shared.Dtos.HocSinh;

namespace StudentManagement.Shared.Dtos.Lop;

public class LopDetailResponse
{
    public string IdLop { get; set; } = string.Empty;
    public string TenLop { get; set; } = string.Empty;
    public int SiSo { get; set; }
    public List<HocSinhResponse> HocSinhs { get; set; } = new();
}
