using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.Admin;
using StudentManagement.Shared.Dtos.Common;

namespace StudentManagement.Desktop.Services;

public sealed class HistoryApiClient : IHistoryApiClient
{
    private readonly HttpClient _httpClient;

    public HistoryApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<HistoryDto>> SearchLogsAsync(
        string? keyword,
        string? hanhDong,
        DateTime? tuNgay,
        DateTime? denNgay,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrWhiteSpace(keyword))
            queryParams.Add($"keyword={Uri.EscapeDataString(keyword)}");
        if (!string.IsNullOrWhiteSpace(hanhDong))
            queryParams.Add($"hanhDong={Uri.EscapeDataString(hanhDong)}");
        if (tuNgay.HasValue)
            queryParams.Add($"tuNgay={tuNgay.Value:yyyy-MM-dd}");
        if (denNgay.HasValue)
            queryParams.Add($"denNgay={denNgay.Value:yyyy-MM-dd}");

        string url = "api/lichsu/search";
        if (queryParams.Count > 0)
        {
            url += "?" + string.Join("&", queryParams);
        }

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<List<HistoryDto>>(cancellationToken))!;
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
