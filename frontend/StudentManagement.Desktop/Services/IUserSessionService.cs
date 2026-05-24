using StudentManagement.Desktop.Models;
using StudentManagement.Shared.Dtos.Auth;

namespace StudentManagement.Desktop.Services;

public interface IUserSessionService
{
    UserSession? CurrentUser { get; }
    ProfileResponse? Profile { get; set; }
    bool IsAuthenticated { get; }
    void SetCurrentUser(UserSession session);
    void Clear();
}
