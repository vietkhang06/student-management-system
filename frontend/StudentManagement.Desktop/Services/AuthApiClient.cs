using System.Net.Http;
using System.Net.Http.Json;
using StudentManagement.Shared.Dtos.Auth;
using StudentManagement.Shared.Dtos.Common;

namespace StudentManagement.Desktop.Services;

public sealed class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient _httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/auth/login", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            ApiErrorResponse? error = null;

            try
            {
                error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken);
            }
            catch
            {
                // Fall through to the generic message when the server returns a non-JSON response.
            }

            throw new ApiException(error?.Message ?? "Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin !");
        }

        AuthResponse? result = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken);
        return result ?? throw new ApiException("Server không trả về dữ liệu đăng nhập hợp lệ.");
    }

    public async Task<ProfileResponse> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync("api/auth/me", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            ApiErrorResponse? error = null;
            try
            {
                error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken);
            }
            catch
            {
                // Ignored
            }

            throw new ApiException(error?.Message ?? "Lỗi tải thông tin cá nhân.");
        }

        ProfileResponse? profile = await response.Content.ReadFromJsonAsync<ProfileResponse>(cancellationToken);
        return profile ?? throw new ApiException("Server không trả về dữ liệu đăng nhập hợp lệ.");
    }
}
