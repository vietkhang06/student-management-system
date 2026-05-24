using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using StudentManagement.Desktop.Services;
using StudentManagement.Shared.Dtos.HocSinh;

namespace StudentManagement.Desktop.ViewModels;

public sealed partial class ClassDetailViewModel : ObservableObject
{
    private readonly IClassApiClient _classApiClient;
    private readonly ShellViewModel _shellViewModel;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private string _classId = string.Empty;

    [ObservableProperty]
    private string _className = string.Empty;

    [ObservableProperty]
    private int _siSo;

    [ObservableProperty]
    private bool _isBusy;

    public ObservableCollection<ClassStudentGridItem> StudentList { get; } = new();

    public ClassDetailViewModel(
        IClassApiClient classApiClient,
        ShellViewModel shellViewModel,
        IServiceProvider serviceProvider)
    {
        _classApiClient = classApiClient;
        _shellViewModel = shellViewModel;
        _serviceProvider = serviceProvider;
    }

    public void Initialize(string classId)
    {
        ClassId = classId;
        _ = LoadDetailsAsync();
    }

    private async Task LoadDetailsAsync()
    {
        IsBusy = true;
        try
        {
            StudentList.Clear();
            var detail = await _classApiClient.GetByIdAsync(ClassId);
            ClassName = detail.TenLop;
            SiSo = detail.SiSo;

            foreach (var student in detail.HocSinhs)
            {
                StudentList.Add(new ClassStudentGridItem
                {
                    IdHocSinh = student.IdHocSinh,
                    Ten = student.Ten,
                    GioiTinh = student.GioiTinh,
                    NamSinh = student.NgaySinh.HasValue ? student.NgaySinh.Value.Year.ToString() : "-",
                    DiaChi = student.DiaChi
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải thông tin chi tiết lớp học: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Back()
    {
        _shellViewModel.CurrentPageViewModel = _serviceProvider.GetRequiredService<ClassListViewModel>();
    }
}

public sealed class ClassStudentGridItem
{
    public string IdHocSinh { get; init; } = string.Empty;
    public string Ten { get; init; } = string.Empty;
    public string GioiTinh { get; init; } = string.Empty;
    public string NamSinh { get; init; } = string.Empty;
    public string DiaChi { get; init; } = string.Empty;
}
