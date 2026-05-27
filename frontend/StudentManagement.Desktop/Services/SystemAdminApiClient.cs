using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.Admin;
using StudentManagement.Shared.Dtos.MonHoc;
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

    public async Task<SystemHealthResponse> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync("api/admin/health", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<SystemHealthResponse>(cancellationToken))!;
    }

    public async Task<List<GiaoVienResponse>> GetTeachersAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync("api/admin/teachers", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<List<GiaoVienResponse>>(cancellationToken))!;
    }

    public async Task<GiaoVienResponse> CreateTeacherAsync(TeacherCreateRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/admin/teachers", request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<GiaoVienResponse>(cancellationToken))!;
    }

    public async Task<GiaoVienResponse> UpdateTeacherAsync(string idGiaoVien, TeacherUpdateRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsJsonAsync($"api/admin/teachers/{idGiaoVien}", request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<GiaoVienResponse>(cancellationToken))!;
    }

    public async Task DeleteTeacherAsync(string idGiaoVien, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.DeleteAsync($"api/admin/teachers/{idGiaoVien}", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<List<MonHocResponse>> GetSubjectsAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync("api/admin/subjects", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<List<MonHocResponse>>(cancellationToken))!;
    }

    public async Task<MonHocResponse> CreateSubjectAsync(SubjectRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/admin/subjects", request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<MonHocResponse>(cancellationToken))!;
    }

    public async Task<MonHocResponse> UpdateSubjectAsync(string idMonHoc, SubjectRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsJsonAsync($"api/admin/subjects/{idMonHoc}", request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<MonHocResponse>(cancellationToken))!;
    }

    public async Task DeleteSubjectAsync(string idMonHoc, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.DeleteAsync($"api/admin/subjects/{idMonHoc}", cancellationToken);
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
