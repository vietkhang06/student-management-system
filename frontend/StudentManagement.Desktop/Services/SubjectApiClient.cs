using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.Common;
using StudentManagement.Shared.Dtos.MonHoc;

namespace StudentManagement.Desktop.Services;

public sealed class SubjectApiClient : ISubjectApiClient
{
    private readonly HttpClient _httpClient;

    public SubjectApiClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<MonHocResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync("api/mon-hoc", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<List<MonHocResponse>>(cancellationToken))!;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode) return;
        ApiErrorResponse? error = null;
        try { error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken); } catch { }
        throw new ApiException(error?.Message ?? $"Lỗi hệ thống: {response.StatusCode}");
    }
}
