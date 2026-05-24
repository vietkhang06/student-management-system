using System;

namespace StudentManagement.Shared.Dtos.HocSinh;

public class HocSinhResponse
{
    public string IdHocSinh { get; set; } = string.Empty;
    public string IdLop { get; set; } = string.Empty;
    public string Ten { get; set; } = string.Empty;
    public string GioiTinh { get; set; } = string.Empty;
    public DateTime? NgaySinh { get; set; }
    public string DiaChi { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
