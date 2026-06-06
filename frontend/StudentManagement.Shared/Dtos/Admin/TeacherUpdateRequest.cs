namespace StudentManagement.Shared.Dtos.Admin;

public sealed class TeacherUpdateRequest
{
    public string Ten { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
    public string GioiTinh { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Sdt { get; set; } = string.Empty;
    public string IdLop { get; set; } = string.Empty;
    public string IdLopChuNhiem { get; set; } = string.Empty;
    public string IdMonHoc { get; set; } = string.Empty;
    public bool Active { get; set; } = true;
}
