using CommunityToolkit.Mvvm.ComponentModel;

namespace StudentManagement.Desktop.ViewModels;

public sealed partial class PlaceholderViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "Tính năng đang được phát triển";

    [ObservableProperty]
    private string _description = "Nghiệp vụ này sẽ được hoàn thiện trong các phase tiếp theo của dự án.";

    public void SetInfo(string title, string description)
    {
        Title = title;
        Description = description;
    }
}
