using System;
using System.Collections.Generic;

namespace StudentManagement.Shared.Dtos.Auth;

public sealed class ProfileResponse
{
    public string IdTaiKhoan { get; set; } = string.Empty;
    public string TenDangNhap { get; set; } = string.Empty;
    public string LoaiTaiKhoan { get; set; } = string.Empty;
    public string Ten { get; set; } = string.Empty;
    public string Cmnd { get; set; } = string.Empty;
    public string Sdt { get; set; } = string.Empty;
    public string? NgaySinh { get; set; }
    public string GioiTinh { get; set; } = string.Empty;
    public string? ChuNhiemLopId { get; set; }
    public List<string> PhanCongKeys { get; set; } = new();
}
