using CommunityToolkit.Mvvm.ComponentModel;
using StudentManagement.Desktop.Services;

namespace StudentManagement.Desktop.ViewModels;

public sealed partial class DashboardViewModel : ObservableObject
{
    private readonly IUserSessionService _userSessionService;

    public DashboardViewModel(IUserSessionService userSessionService)
    {
        _userSessionService = userSessionService;
    }

    public string TenDangNhap => _userSessionService.CurrentUser?.TenDangNhap ?? string.Empty;
    public string VaiTro => _userSessionService.CurrentUser?.LoaiTaiKhoan ?? string.Empty;
    public bool IsBanQuanLy => _userSessionService.CurrentUser?.IsBanQuanLy == true;
    public bool IsGiaoVien => _userSessionService.CurrentUser?.IsGiaoVien == true;

    public bool CanReceiveStudents => IsBanQuanLy;
    public bool CanAccessScores => IsBanQuanLy || IsGiaoVien;
    public bool CanAccessReports => IsBanQuanLy || IsGiaoVien;
    public bool CanManageRegulations => IsBanQuanLy;
    public bool CanManageSystem => IsBanQuanLy;
}
