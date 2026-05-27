using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.HocSinh;
using StudentManagement.Shared.Dtos.Common;

namespace StudentManagement.Desktop.Services;

public sealed class StudentApiClient : IStudentApiClient
{
    private readonly HttpClient _httpClient;

    public StudentApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<HocSinhResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync("api/hoc-sinh", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<List<HocSinhResponse>>(cancellationToken))!;
    }

    public async Task<HocSinhResponse> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"api/hoc-sinh/{id}", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<HocSinhResponse>(cancellationToken))!;
    }

    public async Task<List<HocSinhResponse>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"api/hoc-sinh/search?ten={Uri.EscapeDataString(name)}", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<List<HocSinhResponse>>(cancellationToken))!;
    }

    public async Task<List<HocSinhSummaryResponse>> GetStudentSummariesAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync("api/hoc-sinh/summary", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<List<HocSinhSummaryResponse>>(cancellationToken))!;
    }

    public async Task<HocSinhResponse> CreateAsync(HocSinhCreateRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/hoc-sinh", request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<HocSinhResponse>(cancellationToken))!;
    }

    public async Task<HocSinhResponse> UpdateAsync(string id, HocSinhUpdateRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsJsonAsync($"api/hoc-sinh/{id}", request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<HocSinhResponse>(cancellationToken))!;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.DeleteAsync($"api/hoc-sinh/{id}", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<bool> HasScoresAsync(string id, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"api/hoc-sinh/{id}/has-scores", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<bool>(cancellationToken);
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
