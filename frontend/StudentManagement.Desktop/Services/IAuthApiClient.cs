using StudentManagement.Shared.Dtos.Auth;

namespace StudentManagement.Desktop.Services;

public interface IAuthApiClient
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<ProfileResponse> GetProfileAsync(CancellationToken cancellationToken = default);
}
