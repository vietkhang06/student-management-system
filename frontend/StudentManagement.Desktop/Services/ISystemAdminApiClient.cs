using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.Admin;

namespace StudentManagement.Desktop.Services;

public interface ISystemAdminApiClient
{
    Task<SystemOverviewResponse> GetOverviewAsync(CancellationToken cancellationToken = default);
    Task<PhanCongResponse> AddAssignmentAsync(PhanCongRequest request, CancellationToken cancellationToken = default);
    Task DeleteAssignmentAsync(string idPhanCong, CancellationToken cancellationToken = default);
}
