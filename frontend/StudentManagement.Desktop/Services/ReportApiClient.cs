using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.BaoCao;
using StudentManagement.Shared.Dtos.Common;

namespace StudentManagement.Desktop.Services;

public sealed class ReportApiClient : IReportApiClient
{
    private readonly HttpClient _httpClient;

    public ReportApiClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<BaoCaoMonResponse> GetSubjectReportAsync(
        string idLop, string idMonHoc, string idHocKy,
        CancellationToken cancellationToken = default)
    {
        var url = $"api/bao-cao/tong-ket-mon" +
                  $"?idLop={Uri.EscapeDataString(idLop)}" +
                  $"&idMonHoc={Uri.EscapeDataString(idMonHoc)}" +
                  $"&idHocKy={Uri.EscapeDataString(idHocKy)}";

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<BaoCaoMonResponse>(cancellationToken))!;
    }

    public async Task<BaoCaoHocKyResponse> GetSemesterReportAsync(
        string idLop, string idHocKy,
        CancellationToken cancellationToken = default)
    {
        var url = $"api/bao-cao/tong-ket-hoc-ky" +
                  $"?idLop={Uri.EscapeDataString(idLop)}" +
                  $"&idHocKy={Uri.EscapeDataString(idHocKy)}";

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<BaoCaoHocKyResponse>(cancellationToken))!;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode) return;
        ApiErrorResponse? error = null;
        try { error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken); } catch { }
        throw new ApiException(error?.Message ?? $"Lỗi hệ thống: {response.StatusCode}");
    }
}
