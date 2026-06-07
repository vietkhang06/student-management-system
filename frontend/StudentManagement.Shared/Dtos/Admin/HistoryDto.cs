using System;

namespace StudentManagement.Shared.Dtos.Admin;

public sealed class HistoryDto
{
    public long Id { get; set; }
    public DateTime ThoiGian { get; set; }
    public string NguoiThucHien { get; set; } = string.Empty;
    public string HanhDong { get; set; } = string.Empty;
    public string ChiTiet { get; set; } = string.Empty;
}
