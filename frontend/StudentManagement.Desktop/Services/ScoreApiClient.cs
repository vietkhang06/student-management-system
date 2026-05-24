using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.Common;
using StudentManagement.Shared.Dtos.Diem;

namespace StudentManagement.Desktop.Services;

public sealed class ScoreApiClient : IScoreApiClient
{
    private readonly HttpClient _httpClient;

    public ScoreApiClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<DiemResponse>> GetByClassSubjectTermAsync(
        string idLop, string idMonHoc, string idHocKy,
        CancellationToken cancellationToken = default)
    {
        var url = $"api/diem?idLop={Uri.EscapeDataString(idLop)}" +
                  $"&idMonHoc={Uri.EscapeDataString(idMonHoc)}" +
                  $"&idHocKy={Uri.EscapeDataString(idHocKy)}";

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<List<DiemResponse>>(cancellationToken))!;
    }

    public async Task<DiemResponse> SaveAsync(DiemCreateRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/diem", request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<DiemResponse>(cancellationToken))!;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode) return;
        ApiErrorResponse? error = null;
        try { error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken); } catch { }
        throw new ApiException(error?.Message ?? $"Lỗi hệ thống: {response.StatusCode}");
    }
}
