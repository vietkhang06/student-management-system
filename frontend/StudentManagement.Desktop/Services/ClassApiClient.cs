using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.Lop;
using StudentManagement.Shared.Dtos.Common;

namespace StudentManagement.Desktop.Services;

public sealed class ClassApiClient : IClassApiClient
{
    private readonly HttpClient _httpClient;

    public ClassApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<LopResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync("api/lop", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<List<LopResponse>>(cancellationToken))!;
    }

    public async Task<LopDetailResponse> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"api/lop/{id}", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<LopDetailResponse>(cancellationToken))!;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode) return;

        ApiErrorResponse? error = null;
        try
        {
            error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken);
        }
        catch
        {
            // Ignored
        }

        throw new ApiException(error?.Message ?? $"Lỗi hệ thống: {response.StatusCode}");
    }
}
