using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.BaoCao;

namespace StudentManagement.Desktop.Services;

public interface IReportApiClient
{
    Task<BaoCaoMonResponse> GetSubjectReportAsync(
        string idLop, string idMonHoc, string idHocKy,
        CancellationToken cancellationToken = default);

    Task<BaoCaoHocKyResponse> GetSemesterReportAsync(
        string idLop, string idHocKy,
        CancellationToken cancellationToken = default);
}
