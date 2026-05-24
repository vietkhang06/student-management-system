using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.Diem;

namespace StudentManagement.Desktop.Services;

public interface IScoreApiClient
{
    Task<List<DiemResponse>> GetByClassSubjectTermAsync(
        string idLop, string idMonHoc, string idHocKy,
        CancellationToken cancellationToken = default);

    Task<DiemResponse> SaveAsync(DiemCreateRequest request, CancellationToken cancellationToken = default);
}
