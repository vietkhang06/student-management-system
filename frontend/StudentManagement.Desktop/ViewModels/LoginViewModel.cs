using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentManagement.Desktop.Models;
using StudentManagement.Desktop.Services;
using StudentManagement.Shared.Dtos.Auth;

namespace StudentManagement.Desktop.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthApiClient _authApiClient;
    private readonly IUserSessionService _userSessionService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string tenDangNhap = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string matKhau = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormEnabled))]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private bool isBusy;

    [ObservableProperty]
    private bool isPasswordVisible;

    [ObservableProperty]
    private bool isRememberMeChecked;

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordVisible = !IsPasswordVisible;
    }

    public LoginViewModel(
        IAuthApiClient authApiClient,
        IUserSessionService userSessionService,
        INavigationService navigationService)
    {
        _authApiClient = authApiClient;
        _userSessionService = userSessionService;
        _navigationService = navigationService;
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool IsFormEnabled => !IsBusy;

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        IsBusy = true;

        try
        {
            var response = await _authApiClient.LoginAsync(new LoginRequest
            {
                TenDangNhap = TenDangNhap.Trim(),
                MatKhau = MatKhau
            });

            if (string.IsNullOrWhiteSpace(response.IdTaiKhoan)
                || string.IsNullOrWhiteSpace(response.TenDangNhap)
                || string.IsNullOrWhiteSpace(response.LoaiTaiKhoan)
                || string.IsNullOrWhiteSpace(response.Token))
            {
                throw new ApiException("Server khong tra ve du lieu phien dang nhap hop le.");
            }

            if (!IsSupportedRole(response.LoaiTaiKhoan))
            {
                throw new ApiException("Tai khoan khong co vai tro hop le de su dung ung dung.");
            }

            _userSessionService.SetCurrentUser(new UserSession
            {
                IdTaiKhoan = response.IdTaiKhoan,
                TenDangNhap = response.TenDangNhap,
                LoaiTaiKhoan = response.LoaiTaiKhoan,
                Token = response.Token
            });

            var profile = await _authApiClient.GetProfileAsync();
            _userSessionService.Profile = profile;

            _navigationService.ShowShell();
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Khong ket noi duoc API server. Kiem tra backend tai http://localhost:8080.";
        }
        catch (TaskCanceledException)
        {
            ErrorMessage = "Ket noi API qua thoi gian cho. Vui long thu lai.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Dang nhap that bai: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            LoginCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(IsFormEnabled));
        }
    }

    partial void OnErrorMessageChanged(string value)
    {
        OnPropertyChanged(nameof(HasError));
    }

    private bool CanLogin()
    {
        return !IsBusy
            && !string.IsNullOrWhiteSpace(TenDangNhap)
            && !string.IsNullOrWhiteSpace(MatKhau);
    }

    private static bool IsSupportedRole(string role)
    {
        return string.Equals(role, UserRoles.BanQuanLy, StringComparison.OrdinalIgnoreCase)
            || string.Equals(role, UserRoles.GiaoVien, StringComparison.OrdinalIgnoreCase);
    }

    partial void OnTenDangNhapChanged(string value)
    {
        LoginCommand.NotifyCanExecuteChanged();
    }

    partial void OnMatKhauChanged(string value)
    {
        LoginCommand.NotifyCanExecuteChanged();
    }
}
