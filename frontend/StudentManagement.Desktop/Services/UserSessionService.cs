using StudentManagement.Desktop.Models;
using StudentManagement.Shared.Dtos.Auth;

namespace StudentManagement.Desktop.Services;

public sealed class UserSessionService : IUserSessionService
{
    public UserSession? CurrentUser { get; private set; }
    public ProfileResponse? Profile { get; set; }
    public bool IsAuthenticated => CurrentUser is not null;

    public void SetCurrentUser(UserSession session)
    {
        CurrentUser = session;
    }

    public void Clear()
    {
        CurrentUser = null;
        Profile = null;
    }
}
