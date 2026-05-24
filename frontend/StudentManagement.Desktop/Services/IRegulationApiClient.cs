using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.ThamSo;

namespace StudentManagement.Desktop.Services;

public interface IRegulationApiClient
{
    Task<List<ThamSoResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ThamSoResponse> UpdateAsync(string id, ThamSoUpdateRequest request, CancellationToken cancellationToken = default);
}
