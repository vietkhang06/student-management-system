using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.MonHoc;

namespace StudentManagement.Desktop.Services;

public interface ISubjectApiClient
{
    Task<List<MonHocResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}
