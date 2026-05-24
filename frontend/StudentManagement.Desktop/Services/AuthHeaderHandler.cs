using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace StudentManagement.Desktop.Services;

public sealed class AuthHeaderHandler : DelegatingHandler
{
    private readonly IUserSessionService _userSessionService;

    public AuthHeaderHandler(IUserSessionService userSessionService)
    {
        _userSessionService = userSessionService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _userSessionService.CurrentUser?.Token;
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
