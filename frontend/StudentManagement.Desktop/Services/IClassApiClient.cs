using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.Lop;

namespace StudentManagement.Desktop.Services;

public interface IClassApiClient
{
    Task<List<LopResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<LopDetailResponse> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}
