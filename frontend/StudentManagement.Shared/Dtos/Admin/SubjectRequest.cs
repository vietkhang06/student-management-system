namespace StudentManagement.Shared.Dtos.Admin;

public sealed class SubjectRequest
{
    public string IdMonHoc { get; set; } = string.Empty;
    public string TenMonHoc { get; set; } = string.Empty;
    public bool TrangThaiSuDung { get; set; } = true;
}
