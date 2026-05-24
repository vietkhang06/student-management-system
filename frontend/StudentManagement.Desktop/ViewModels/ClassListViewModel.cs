using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using StudentManagement.Desktop.Services;
using StudentManagement.Shared.Dtos.Lop;

namespace StudentManagement.Desktop.ViewModels;

public sealed partial class ClassListViewModel : ObservableObject
{
    private readonly IClassApiClient _classApiClient;
    private readonly ShellViewModel _shellViewModel;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private bool _isBusy;

    public ObservableCollection<LopResponse> ClassList { get; } = new();

    public ClassListViewModel(
        IClassApiClient classApiClient,
        ShellViewModel shellViewModel,
        IServiceProvider serviceProvider)
    {
        _classApiClient = classApiClient;
        _shellViewModel = shellViewModel;
        _serviceProvider = serviceProvider;

        _ = LoadClassesAsync();
    }

    private async Task LoadClassesAsync()
    {
        IsBusy = true;
        try
        {
            ClassList.Clear();
            var classes = await _classApiClient.GetAllAsync();
            foreach (var cls in classes)
            {
                ClassList.Add(cls);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải danh sách lớp học: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ViewDetails(LopResponse selectedClass)
    {
        if (selectedClass is null) return;

        var detailVm = _serviceProvider.GetRequiredService<ClassDetailViewModel>();
        detailVm.Initialize(selectedClass.IdLop);
        _shellViewModel.CurrentPageViewModel = detailVm;
    }
}
