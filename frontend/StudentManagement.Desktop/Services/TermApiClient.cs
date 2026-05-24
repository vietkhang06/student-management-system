using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.Common;
using StudentManagement.Shared.Dtos.HocKy;

namespace StudentManagement.Desktop.Services;

public sealed class TermApiClient : ITermApiClient
{
    private readonly HttpClient _httpClient;

    public TermApiClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<HocKyResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync("api/hoc-ky", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<List<HocKyResponse>>(cancellationToken))!;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode) return;
        ApiErrorResponse? error = null;
        try { error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken); } catch { }
        throw new ApiException(error?.Message ?? $"Lỗi hệ thống: {response.StatusCode}");
    }
}
