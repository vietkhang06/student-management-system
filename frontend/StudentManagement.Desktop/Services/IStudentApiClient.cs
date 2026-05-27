using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.HocSinh;

namespace StudentManagement.Desktop.Services;

public interface IStudentApiClient
{
    Task<List<HocSinhResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<HocSinhResponse> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<List<HocSinhResponse>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<HocSinhSummaryResponse>> GetStudentSummariesAsync(CancellationToken cancellationToken = default);
    Task<HocSinhResponse> CreateAsync(HocSinhCreateRequest request, CancellationToken cancellationToken = default);
    Task<HocSinhResponse> UpdateAsync(string id, HocSinhUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> HasScoresAsync(string id, CancellationToken cancellationToken = default);
}
