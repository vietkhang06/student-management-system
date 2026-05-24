using System.Collections.Generic;

namespace StudentManagement.Shared.Dtos.Admin;

public sealed class SystemOverviewResponse
{
    public long TotalStudents { get; set; }
    public long TotalClasses { get; set; }
    public long TotalTeachers { get; set; }
    public long TotalSubjects { get; set; }
    public List<PhanCongResponse> Assignments { get; set; } = new();
    public List<GiaoVienResponse> Teachers { get; set; } = new();
}
