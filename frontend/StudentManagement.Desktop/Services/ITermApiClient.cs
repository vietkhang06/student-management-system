using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.HocKy;

namespace StudentManagement.Desktop.Services;

public interface ITermApiClient
{
    Task<List<HocKyResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}
