using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentManagement.Desktop.Services;
using StudentManagement.Shared.Dtos.Admin;

namespace StudentManagement.Desktop.ViewModels;

public sealed partial class HistoryViewModel : ObservableObject
{
    private readonly IHistoryApiClient _historyApiClient;

    [ObservableProperty]
    private string _keyword = string.Empty;

    [ObservableProperty]
    private string _selectedAction = "Tất cả";

    [ObservableProperty]
    private DateTime? _tuNgay;

    [ObservableProperty]
    private DateTime? _denNgay;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasStatusMessage))]
    private string _statusMessage = string.Empty;

    public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

    [ObservableProperty]
    private ObservableCollection<HistoryDto> _logs = new();

    public List<string> ActionsList { get; } = new()
    {
        "Tất cả",
        "Thêm học sinh",
        "Sửa học sinh",
        "Xóa học sinh",
        "Thêm giáo viên",
        "Sửa giáo viên",
        "Xóa giáo viên",
        "Cập nhật điểm"
    };

    public HistoryViewModel(IHistoryApiClient historyApiClient)
    {
        _historyApiClient = historyApiClient;
        _ = LoadLogsAsync();
    }

    [RelayCommand]
    private async Task LoadLogsAsync()
    {
        if (IsLoading) return;

        IsLoading = true;
        StatusMessage = string.Empty;

        try
        {
            var results = await _historyApiClient.SearchLogsAsync(Keyword, SelectedAction, TuNgay, DenNgay);
            Logs.Clear();
            foreach (var log in results)
            {
                Logs.Add(log);
            }

            if (Logs.Count == 0)
            {
                StatusMessage = "Không tìm thấy nhật ký hoạt động nào phù hợp.";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải lịch sử hệ thống: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            StatusMessage = "Lỗi khi tải dữ liệu từ máy chủ.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ResetFiltersAsync()
    {
        Keyword = string.Empty;
        SelectedAction = "Tất cả";
        TuNgay = null;
        DenNgay = null;
        await LoadLogsAsync();
    }
}
