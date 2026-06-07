using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentManagement.Desktop.Models;
using StudentManagement.Desktop.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace StudentManagement.Desktop.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    private readonly IUserSessionService _userSessionService;
    private readonly INavigationService _navigationService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private object? _currentPageViewModel;

    public ShellViewModel(
        IUserSessionService userSessionService,
        INavigationService navigationService,
        IServiceProvider serviceProvider)
    {
        _userSessionService = userSessionService;
        _navigationService = navigationService;
        _serviceProvider = serviceProvider;

        // Navigate to dashboard by default
        NavigateToDashboard();
    }

    public string TenDangNhap => CurrentUser?.TenDangNhap ?? string.Empty;
    public string VaiTro => CurrentUser?.LoaiTaiKhoan ?? string.Empty;
    public string TenNguoiDung => _userSessionService.Profile?.Ten ?? TenDangNhap;
    
    public string Initials
    {
        get
        {
            string name = TenNguoiDung;
            if (string.IsNullOrWhiteSpace(name)) return "U";
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                string initials = parts.Length == 1 
                    ? parts[0][0].ToString() 
                    : parts[0][0].ToString() + parts[parts.Length - 1][0].ToString();
                return initials.ToUpper();
            }
            return "U";
        }
    }

    public string VaiTroHienThi => string.Equals(VaiTro, "BANQUANLY", StringComparison.OrdinalIgnoreCase) ? "Ban Quản Lý" : "Giáo Viên";
    public bool IsBanQuanLy => CurrentUser?.IsBanQuanLy == true;
    public bool IsGiaoVien => CurrentUser?.IsGiaoVien == true;
    public bool CanReceiveStudents => IsBanQuanLy;
    public bool CanManageRegulations => IsBanQuanLy;
    public bool CanManageSystem => IsBanQuanLy;
    public bool CanAccessScores => IsBanQuanLy || IsGiaoVien;
    public bool CanAccessReports => IsBanQuanLy || IsGiaoVien;

    private UserSession? CurrentUser => _userSessionService.CurrentUser;

    [RelayCommand]
    public void NavigateToDashboard()
    {
        CurrentPageViewModel = _serviceProvider.GetRequiredService<DashboardViewModel>();
    }

    [RelayCommand]
    public void NavigateToStudentList()
    {
        CurrentPageViewModel = _serviceProvider.GetRequiredService<StudentListViewModel>();
    }

    [RelayCommand]
    public void NavigateToClassList()
    {
        CurrentPageViewModel = _serviceProvider.GetRequiredService<ClassListViewModel>();
    }

    [RelayCommand]
    public void NavigateToAdmission()
    {
        CurrentPageViewModel = _serviceProvider.GetRequiredService<AdmissionViewModel>();
    }

    [RelayCommand]
    public void NavigateToScores()
    {
        CurrentPageViewModel = _serviceProvider.GetRequiredService<ScoreViewModel>();
    }

    [RelayCommand]
    public void NavigateToReports()
    {
        CurrentPageViewModel = _serviceProvider.GetRequiredService<ReportViewModel>();
    }

    [RelayCommand]
    public void NavigateToRegulations()
    {
        CurrentPageViewModel = _serviceProvider.GetRequiredService<RegulationViewModel>();
    }

    [RelayCommand]
    public void NavigateToSystemAdmin()
    {
        CurrentPageViewModel = _serviceProvider.GetRequiredService<SystemAdminViewModel>();
    }

    [RelayCommand]
    public void NavigateToHistory()
    {
        CurrentPageViewModel = _serviceProvider.GetRequiredService<HistoryViewModel>();
    }

    [RelayCommand]
    private void Logout()
    {
        _userSessionService.Clear();
        _navigationService.ShowLogin();
    }

    [RelayCommand]
    public void ShowProfile()
    {
        var profile = _userSessionService.Profile;
        if (profile == null) return;

        var profileWindow = new Views.ProfileWindow(profile);
        profileWindow.Owner = System.Windows.Application.Current.MainWindow;
        profileWindow.ShowDialog();
    }
}
