using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentManagement.Desktop.Services;
using StudentManagement.Desktop.Views;
using StudentManagement.Shared.Dtos.HocSinh;
using StudentManagement.Shared.Dtos.Lop;

namespace StudentManagement.Desktop.ViewModels;

public sealed partial class StudentListViewModel : ObservableObject
{
    private readonly IStudentApiClient _studentApiClient;
    private readonly IClassApiClient _classApiClient;
    private readonly IUserSessionService _sessionService;
    private readonly IConfirmationService _confirmationService;

    [ObservableProperty]
    private string _searchName = string.Empty;

    [ObservableProperty]
    private LopResponse? _selectedClass;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsStudentSelected))]
    private StudentGridItem? _selectedStudent;

    public bool IsStudentSelected => SelectedStudent != null;

    public bool IsBanQuanLy => _sessionService.CurrentUser?.IsBanQuanLy == true;

    public ObservableCollection<LopResponse> ClassList { get; } = new();
    public ObservableCollection<StudentGridItem> StudentList { get; } = new();

    private readonly List<HocSinhSummaryResponse> _allStudentSummaries = new();

    public StudentListViewModel(
        IStudentApiClient studentApiClient,
        IClassApiClient classApiClient,
        IUserSessionService sessionService,
        IConfirmationService confirmationService)
    {
        _studentApiClient = studentApiClient;
        _classApiClient = classApiClient;
        _sessionService = sessionService;
        _confirmationService = confirmationService;

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            ClassList.Clear();
            _allStudentSummaries.Clear();

            var classes = await _classApiClient.GetAllAsync();
            foreach (var cls in classes)
                ClassList.Add(cls);

            var summaries = await _studentApiClient.GetStudentSummariesAsync();
            _allStudentSummaries.AddRange(summaries);

            ApplyFilters();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải danh sách học sinh: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Search() => ApplyFilters();

    [RelayCommand]
    private void Reset()
    {
        SearchName = string.Empty;
        SelectedClass = null;
        ApplyFilters();
    }

    [RelayCommand]
    private async Task AddStudentAsync()
    {
        var dialog = new StudentWindow(ClassList.ToList());
        dialog.Owner = Application.Current.MainWindow;

        if (dialog.ShowDialog() != true || dialog.CreateResult == null)
            return;

        IsBusy = true;
        try
        {
            await _studentApiClient.CreateAsync(dialog.CreateResult);
            await LoadDataAsync();
            MessageBox.Show("Thêm học sinh thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi thêm học sinh: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task EditStudentAsync()
    {
        if (SelectedStudent == null) return;

        HocSinhResponse? detail;
        IsBusy = true;
        try
        {
            detail = await _studentApiClient.GetByIdAsync(SelectedStudent.IdHocSinh);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải thông tin học sinh: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            IsBusy = false;
            return;
        }
        finally
        {
            IsBusy = false;
        }

        var dialog = new StudentWindow(ClassList.ToList(), detail);
        dialog.Owner = Application.Current.MainWindow;

        if (dialog.ShowDialog() != true || dialog.UpdateResult == null)
            return;

        IsBusy = true;
        try
        {
            await _studentApiClient.UpdateAsync(SelectedStudent.IdHocSinh, dialog.UpdateResult);
            await LoadDataAsync();
            MessageBox.Show("Cập nhật học sinh thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi cập nhật học sinh: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeleteStudentAsync()
    {
        if (SelectedStudent == null) return;

        var studentName = SelectedStudent.Ten;
        var studentId = SelectedStudent.IdHocSinh;

        // Check if the student has scores
        bool hasScores;
        IsBusy = true;
        try
        {
            hasScores = await _studentApiClient.HasScoresAsync(studentId);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi kiểm tra dữ liệu học sinh: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            IsBusy = false;
            return;
        }
        finally
        {
            IsBusy = false;
        }

        string confirmMessage;
        if (hasScores)
        {
            confirmMessage = $"Học sinh \"{studentName}\" có dữ liệu điểm. Xóa sẽ đồng thời xóa toàn bộ điểm số liên quan.\n\nBạn có chắc chắn muốn xóa không?";
        }
        else
        {
            confirmMessage = $"Bạn có chắc chắn muốn xóa học sinh \"{studentName}\"?";
        }

        var confirmed = await _confirmationService.ConfirmActionAsync("DELETE_STUDENT", confirmMessage);
        if (!confirmed) return;

        IsBusy = true;
        try
        {
            await _studentApiClient.DeleteAsync(studentId);
            await LoadDataAsync();
            MessageBox.Show("Đã xóa học sinh thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi xóa học sinh: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync() => await LoadDataAsync();

    private void ApplyFilters()
    {
        StudentList.Clear();

        IEnumerable<HocSinhSummaryResponse> query = _allStudentSummaries;

        if (!string.IsNullOrWhiteSpace(SearchName))
            query = query.Where(s => s.Ten.Contains(SearchName, StringComparison.OrdinalIgnoreCase));

        if (SelectedClass is not null)
            query = query.Where(s => s.IdLop == SelectedClass.IdLop);

        var classMap = ClassList.ToDictionary(c => c.IdLop, c => c.TenLop);

        foreach (var item in query)
        {
            classMap.TryGetValue(item.IdLop, out var className);
            StudentList.Add(new StudentGridItem
            {
                IdHocSinh = item.IdHocSinh,
                Ten = item.Ten,
                TenLop = className ?? item.IdLop,
                TbHocKy1 = item.TbHocKy1,
                TbHocKy2 = item.TbHocKy2
            });
        }
    }
}

public sealed class StudentGridItem
{
    public string IdHocSinh { get; init; } = string.Empty;
    public string Ten { get; init; } = string.Empty;
    public string TenLop { get; init; } = string.Empty;
    public double? TbHocKy1 { get; init; }
    public double? TbHocKy2 { get; init; }

    public string TbHocKy1Display => TbHocKy1.HasValue ? TbHocKy1.Value.ToString("F1") : "-";
    public string TbHocKy2Display => TbHocKy2.HasValue ? TbHocKy2.Value.ToString("F1") : "-";
}
