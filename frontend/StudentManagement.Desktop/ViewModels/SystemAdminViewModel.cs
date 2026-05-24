using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentManagement.Desktop.Services;
using StudentManagement.Shared.Dtos.Admin;
using StudentManagement.Shared.Dtos.HocKy;
using StudentManagement.Shared.Dtos.Lop;
using StudentManagement.Shared.Dtos.MonHoc;

namespace StudentManagement.Desktop.ViewModels;

public sealed partial class SystemAdminViewModel : ObservableObject
{
    private readonly ISystemAdminApiClient _adminApiClient;
    private readonly IClassApiClient _classApiClient;
    private readonly ISubjectApiClient _subjectApiClient;
    private readonly ITermApiClient _termApiClient;
    private readonly IConfirmationService _confirmationService;

    // ── Counters ───────────────────────────────────────────────────────────
    [ObservableProperty]
    private long _totalStudents;

    [ObservableProperty]
    private long _totalClasses;

    [ObservableProperty]
    private long _totalTeachers;

    [ObservableProperty]
    private long _totalSubjects;

    // ── State ──────────────────────────────────────────────────────────────
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    // ── Collections ────────────────────────────────────────────────────────
    public ObservableCollection<PhanCongResponse> Assignments { get; } = new();
    public ObservableCollection<GiaoVienResponse> Teachers { get; } = new();
    public ObservableCollection<LopResponse> Classes { get; } = new();
    public ObservableCollection<MonHocResponse> Subjects { get; } = new();
    public ObservableCollection<HocKyResponse> Terms { get; } = new();

    // ── Form fields ────────────────────────────────────────────────────────
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddAssignmentCommand))]
    private GiaoVienResponse? _selectedTeacher;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddAssignmentCommand))]
    private LopResponse? _selectedClass;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddAssignmentCommand))]
    private MonHocResponse? _selectedSubject;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddAssignmentCommand))]
    private HocKyResponse? _selectedTerm;

    public SystemAdminViewModel(
        ISystemAdminApiClient adminApiClient,
        IClassApiClient classApiClient,
        ISubjectApiClient subjectApiClient,
        ITermApiClient termApiClient,
        IConfirmationService confirmationService)
    {
        _adminApiClient = adminApiClient;
        _classApiClient = classApiClient;
        _subjectApiClient = subjectApiClient;
        _termApiClient = termApiClient;
        _confirmationService = confirmationService;

        _ = LoadDataAsync();
    }

    [RelayCommand]
    private async Task Refresh() => await LoadDataAsync();

    private bool CanAddAssignment =>
        SelectedTeacher != null &&
        SelectedClass != null &&
        SelectedSubject != null &&
        SelectedTerm != null &&
        !IsLoading;

    [RelayCommand(CanExecute = nameof(CanAddAssignment))]
    private async Task AddAssignment()
    {
        StatusMessage = string.Empty;

        if (SelectedTeacher == null || SelectedClass == null || SelectedSubject == null || SelectedTerm == null) return;

        var confirmed = await _confirmationService.ConfirmActionAsync(
            "system_admin",
            $"Bạn có chắc chắn muốn phân công giáo viên {SelectedTeacher.TenGiaoVien} dạy môn {SelectedSubject.TenMonHoc} tại lớp {SelectedClass.TenLop} không?"
        );
        if (!confirmed) return;

        IsLoading = true;
        try
        {
            var request = new PhanCongRequest
            {
                IdGiaoVien = SelectedTeacher.IdGiaoVien,
                IdLop = SelectedClass.IdLop,
                IdMonHoc = SelectedSubject.IdMonHoc,
                IdHocKy = SelectedTerm.IdHocKy
            };

            var created = await _adminApiClient.AddAssignmentAsync(request);
            Assignments.Add(created);

            SelectedTeacher = null;
            SelectedClass = null;
            SelectedSubject = null;
            SelectedTerm = null;

            StatusMessage = "✅ Thêm phân công giảng dạy thành công!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Lỗi: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAssignment(PhanCongResponse pc)
    {
        if (pc == null) return;

        var confirmed = await _confirmationService.ConfirmActionAsync(
            "system_admin",
            $"Bạn có chắc chắn muốn xóa phân công giảng dạy của giáo viên {pc.TenGiaoVien} tại lớp {pc.TenLop} không?"
        );
        if (!confirmed) return;

        IsLoading = true;
        StatusMessage = string.Empty;
        try
        {
            await _adminApiClient.DeleteAssignmentAsync(pc.IdPhanCong);
            Assignments.Remove(pc);
            StatusMessage = "✅ Đã xóa phân công giảng dạy thành công!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Lỗi khi xóa: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        StatusMessage = string.Empty;
        Assignments.Clear();
        Teachers.Clear();
        Classes.Clear();
        Subjects.Clear();
        Terms.Clear();

        try
        {
            // ── Load lookups and counts in parallel ─────────────────────────
            var overviewTask = _adminApiClient.GetOverviewAsync();
            var classTask = _classApiClient.GetAllAsync();
            var subjectTask = _subjectApiClient.GetAllAsync();
            var termTask = _termApiClient.GetAllAsync();

            await Task.WhenAll(overviewTask, classTask, subjectTask, termTask);

            var overview = await overviewTask;
            TotalStudents = overview.TotalStudents;
            TotalClasses = overview.TotalClasses;
            TotalTeachers = overview.TotalTeachers;
            TotalSubjects = overview.TotalSubjects;

            foreach (var pc in overview.Assignments) Assignments.Add(pc);
            foreach (var t in overview.Teachers) Teachers.Add(t);
            foreach (var c in await classTask) Classes.Add(c);
            foreach (var s in await subjectTask) Subjects.Add(s);
            foreach (var tm in await termTask) Terms.Add(tm);
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Không thể tải thông tin hệ thống: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
