using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.Admin;
using StudentManagement.Shared.Dtos.MonHoc;

namespace StudentManagement.Desktop.Services;

public interface ISystemAdminApiClient
{
    Task<SystemOverviewResponse> GetOverviewAsync(CancellationToken cancellationToken = default);
    Task<PhanCongResponse> AddAssignmentAsync(PhanCongRequest request, CancellationToken cancellationToken = default);
    Task DeleteAssignmentAsync(string idPhanCong, CancellationToken cancellationToken = default);
    Task<SystemHealthResponse> GetHealthAsync(CancellationToken cancellationToken = default);
    
    Task<List<GiaoVienResponse>> GetTeachersAsync(CancellationToken cancellationToken = default);
    Task<GiaoVienResponse> CreateTeacherAsync(TeacherCreateRequest request, CancellationToken cancellationToken = default);
    Task<GiaoVienResponse> UpdateTeacherAsync(string idGiaoVien, TeacherUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteTeacherAsync(string idGiaoVien, CancellationToken cancellationToken = default);
    
    Task<List<MonHocResponse>> GetSubjectsAsync(CancellationToken cancellationToken = default);
    Task<MonHocResponse> CreateSubjectAsync(SubjectRequest request, CancellationToken cancellationToken = default);
    Task<MonHocResponse> UpdateSubjectAsync(string idMonHoc, SubjectRequest request, CancellationToken cancellationToken = default);
    Task DeleteSubjectAsync(string idMonHoc, CancellationToken cancellationToken = default);
}
