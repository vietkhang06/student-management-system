using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.Admin;
using StudentManagement.Shared.Dtos.Common;

namespace StudentManagement.Desktop.Services;

public sealed class SystemAdminApiClient : ISystemAdminApiClient
{
    private readonly HttpClient _httpClient;

    public SystemAdminApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SystemOverviewResponse> GetOverviewAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync("api/admin/overview", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<SystemOverviewResponse>(cancellationToken))!;
    }

    public async Task<PhanCongResponse> AddAssignmentAsync(PhanCongRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/admin/assignment", request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<PhanCongResponse>(cancellationToken))!;
    }

    public async Task DeleteAssignmentAsync(string idPhanCong, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.DeleteAsync($"api/admin/assignment/{idPhanCong}", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
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
