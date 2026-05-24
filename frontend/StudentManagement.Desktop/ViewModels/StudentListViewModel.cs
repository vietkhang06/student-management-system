using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentManagement.Desktop.Services;
using StudentManagement.Shared.Dtos.HocSinh;
using StudentManagement.Shared.Dtos.Lop;

namespace StudentManagement.Desktop.ViewModels;

public sealed partial class StudentListViewModel : ObservableObject
{
    private readonly IStudentApiClient _studentApiClient;
    private readonly IClassApiClient _classApiClient;

    [ObservableProperty]
    private string _searchName = string.Empty;

    [ObservableProperty]
    private LopResponse? _selectedClass;

    [ObservableProperty]
    private bool _isBusy;

    public ObservableCollection<LopResponse> ClassList { get; } = new();
    public ObservableCollection<StudentGridItem> StudentList { get; } = new();

    private readonly List<HocSinhSummaryResponse> _allStudentSummaries = new();

    public StudentListViewModel(IStudentApiClient studentApiClient, IClassApiClient classApiClient)
    {
        _studentApiClient = studentApiClient;
        _classApiClient = classApiClient;

        // Load data on load
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            ClassList.Clear();
            _allStudentSummaries.Clear();

            // Fetch classes
            var classes = await _classApiClient.GetAllAsync();
            foreach (var cls in classes)
            {
                ClassList.Add(cls);
            }

            // Fetch student summaries
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
    private void Search()
    {
        ApplyFilters();
    }

    [RelayCommand]
    private void Reset()
    {
        SearchName = string.Empty;
        SelectedClass = null;
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        StudentList.Clear();

        IEnumerable<HocSinhSummaryResponse> query = _allStudentSummaries;

        if (!string.IsNullOrWhiteSpace(SearchName))
        {
            query = query.Where(s => s.Ten.Contains(SearchName, StringComparison.OrdinalIgnoreCase));
        }

        if (SelectedClass is not null)
        {
            query = query.Where(s => s.IdLop == SelectedClass.IdLop);
        }

        // Map to grid items with class name lookup
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
