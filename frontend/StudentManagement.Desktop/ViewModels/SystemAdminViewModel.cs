using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentManagement.Desktop.Services;
using StudentManagement.Shared.Dtos.Admin;
using StudentManagement.Shared.Dtos.HocKy;
using StudentManagement.Shared.Dtos.Lop;
using StudentManagement.Shared.Dtos.MonHoc;
using StudentManagement.Shared.Dtos.Auth;

namespace StudentManagement.Desktop.ViewModels;

public sealed partial class SystemAdminViewModel : ObservableObject
{
    private readonly ISystemAdminApiClient _adminApiClient;
    private readonly IClassApiClient _classApiClient;
    private readonly ISubjectApiClient _subjectApiClient;
    private readonly ITermApiClient _termApiClient;
    private readonly IConfirmationService _confirmationService;
    private readonly IUserSessionService _userSessionService;

    [ObservableProperty]
    private SystemHealthResponse? _health;

    public ProfileResponse? AdminProfile => _userSessionService.Profile;

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
    [NotifyPropertyChangedFor(nameof(HasStatusMessage))]
    private string _statusMessage = string.Empty;

    public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

    // ── Search & Filter ───────────────────────────────────────────────────
    [ObservableProperty]
    private string _teacherSearchText = string.Empty;

    [ObservableProperty]
    private string _subjectSearchText = string.Empty;

    partial void OnTeacherSearchTextChanged(string value) => ApplyTeacherFilter();
    partial void OnSubjectSearchTextChanged(string value) => ApplySubjectFilter();

    private readonly List<GiaoVienResponse> _allTeachers = new();
    private readonly List<MonHocResponse> _allSubjects = new();

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
        IConfirmationService confirmationService,
        IUserSessionService userSessionService)
    {
        _adminApiClient = adminApiClient;
        _classApiClient = classApiClient;
        _subjectApiClient = subjectApiClient;
        _termApiClient = termApiClient;
        _confirmationService = confirmationService;
        _userSessionService = userSessionService;

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
            "delete_assignment",
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

    // ── TEACHER CRUD COMMANDS ──────────────────────────────────────────────
    [RelayCommand]
    private void ApplyTeacherFilter()
    {
        Teachers.Clear();
        var query = _allTeachers.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(TeacherSearchText))
        {
            var search = TeacherSearchText.Trim();
            query = query.Where(t =>
                t.IdGiaoVien.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                t.TenGiaoVien.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                t.TenDangNhap.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (t.Email != null && t.Email.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                (t.Sdt != null && t.Sdt.Contains(search, StringComparison.OrdinalIgnoreCase))
            );
        }
        foreach (var t in query) Teachers.Add(t);
    }

    [RelayCommand]
    private async Task CreateTeacher()
    {
        var activeSubjects = _allSubjects.Where(s => s.TrangThaiSuDung).ToList();
        var dialog = new Views.TeacherWindow(Classes.ToList(), activeSubjects);
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true && dialog.CreateResult != null)
        {
            IsLoading = true;
            StatusMessage = string.Empty;
            try
            {
                var created = await _adminApiClient.CreateTeacherAsync(dialog.CreateResult);
                _allTeachers.Add(created);
                ApplyTeacherFilter();
                
                // Recount teachers
                TotalTeachers = _allTeachers.Count;
                StatusMessage = "✅ Thêm tài khoản giáo viên thành công!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Lỗi khi thêm: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    [RelayCommand]
    private async Task EditTeacher(GiaoVienResponse teacher)
    {
        if (teacher == null) return;
        var activeSubjects = _allSubjects.Where(s => s.TrangThaiSuDung || s.IdMonHoc == teacher.IdMonHoc).ToList();
        var dialog = new Views.TeacherWindow(Classes.ToList(), activeSubjects, teacher);
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true && dialog.UpdateResult != null)
        {
            IsLoading = true;
            StatusMessage = string.Empty;
            try
            {
                var updated = await _adminApiClient.UpdateTeacherAsync(teacher.IdGiaoVien, dialog.UpdateResult);
                var index = _allTeachers.FindIndex(t => t.IdGiaoVien == teacher.IdGiaoVien);
                if (index >= 0)
                {
                    _allTeachers[index] = updated;
                }
                ApplyTeacherFilter();
                StatusMessage = "✅ Cập nhật thông tin giáo viên thành công!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Lỗi khi cập nhật: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    [RelayCommand]
    private async Task DeleteTeacher(GiaoVienResponse teacher)
    {
        if (teacher == null) return;

        if (AdminProfile != null && string.Equals(teacher.TenDangNhap, AdminProfile.TenDangNhap, StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show("Bạn không thể tự xóa tài khoản quản trị đang đăng nhập của chính mình!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var confirmed = await _confirmationService.ConfirmActionAsync(
            "delete_teacher",
            $"Bạn có chắc chắn muốn xóa tài khoản giáo viên {teacher.TenGiaoVien} (Mã: {teacher.IdGiaoVien}) không? Hành động này cũng sẽ xóa các phân công giảng dạy liên quan."
        );
        if (!confirmed) return;

        IsLoading = true;
        StatusMessage = string.Empty;
        try
        {
            await _adminApiClient.DeleteTeacherAsync(teacher.IdGiaoVien);
            _allTeachers.RemoveAll(t => t.IdGiaoVien == teacher.IdGiaoVien);
            ApplyTeacherFilter();
            
            // Reload assignments in case teaching assignments were deleted cascades
            var overview = await _adminApiClient.GetOverviewAsync();
            Assignments.Clear();
            foreach (var pc in overview.Assignments) Assignments.Add(pc);

            TotalTeachers = _allTeachers.Count;
            StatusMessage = "✅ Xóa tài khoản giáo viên thành công!";
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

    // ── SUBJECT CRUD COMMANDS ──────────────────────────────────────────────
    [RelayCommand]
    private void ApplySubjectFilter()
    {
        Subjects.Clear();
        var query = _allSubjects.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(SubjectSearchText))
        {
            var search = SubjectSearchText.Trim();
            query = query.Where(s =>
                s.IdMonHoc.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                s.TenMonHoc.Contains(search, StringComparison.OrdinalIgnoreCase)
            );
        }
        foreach (var s in query) Subjects.Add(s);
    }

    [RelayCommand]
    private async Task CreateSubject()
    {
        var dialog = new Views.SubjectWindow();
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true && dialog.Result != null)
        {
            IsLoading = true;
            StatusMessage = string.Empty;
            try
            {
                var created = await _adminApiClient.CreateSubjectAsync(dialog.Result);
                _allSubjects.Add(created);
                ApplySubjectFilter();
                TotalSubjects = _allSubjects.Count;
                StatusMessage = "✅ Thêm môn học thành công!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Lỗi khi thêm: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    [RelayCommand]
    private async Task EditSubject(MonHocResponse subject)
    {
        if (subject == null) return;
        var dialog = new Views.SubjectWindow(subject);
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true && dialog.Result != null)
        {
            IsLoading = true;
            StatusMessage = string.Empty;
            try
            {
                var updated = await _adminApiClient.UpdateSubjectAsync(subject.IdMonHoc, dialog.Result);
                var index = _allSubjects.FindIndex(s => s.IdMonHoc == subject.IdMonHoc);
                if (index >= 0)
                {
                    _allSubjects[index] = updated;
                }
                ApplySubjectFilter();
                StatusMessage = "✅ Cập nhật môn học thành công!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Lỗi khi cập nhật: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    [RelayCommand]
    private async Task DeleteSubject(MonHocResponse subject)
    {
        if (subject == null) return;

        var confirmed = await _confirmationService.ConfirmActionAsync(
            "delete_subject",
            $"Bạn có chắc chắn muốn xóa môn học {subject.TenMonHoc} (Mã: {subject.IdMonHoc}) không?"
        );
        if (!confirmed) return;

        IsLoading = true;
        StatusMessage = string.Empty;
        try
        {
            await _adminApiClient.DeleteSubjectAsync(subject.IdMonHoc);
            _allSubjects.RemoveAll(s => s.IdMonHoc == subject.IdMonHoc);
            ApplySubjectFilter();
            TotalSubjects = _allSubjects.Count;
            StatusMessage = "✅ Xóa môn học thành công!";
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
        Health = null;
        Assignments.Clear();
        Teachers.Clear();
        Classes.Clear();
        Subjects.Clear();
        Terms.Clear();
        _allTeachers.Clear();
        _allSubjects.Clear();

        try
        {
            // ── Load lookups, counts, and health in parallel ────────────────
            var overviewTask = _adminApiClient.GetOverviewAsync();
            var classTask = _classApiClient.GetAllAsync();
            var teacherTask = _adminApiClient.GetTeachersAsync();
            var subjectTask = _adminApiClient.GetSubjectsAsync();
            var termTask = _termApiClient.GetAllAsync();
            var healthTask = _adminApiClient.GetHealthAsync();

            await Task.WhenAll(overviewTask, classTask, teacherTask, subjectTask, termTask, healthTask);

            var overview = await overviewTask;
            TotalStudents = overview.TotalStudents;
            TotalClasses = overview.TotalClasses;
            Health = await healthTask;

            foreach (var pc in overview.Assignments) Assignments.Add(pc);
            foreach (var c in await classTask) Classes.Add(c);
            foreach (var tm in await termTask) Terms.Add(tm);

            _allTeachers.AddRange(await teacherTask);
            _allSubjects.AddRange(await subjectTask);

            TotalTeachers = _allTeachers.Count;
            TotalSubjects = _allSubjects.Count;

            ApplyTeacherFilter();
            ApplySubjectFilter();
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
