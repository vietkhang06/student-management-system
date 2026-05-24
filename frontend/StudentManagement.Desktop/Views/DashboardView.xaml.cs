using System.Windows.Controls;
using StudentManagement.Desktop.ViewModels;

namespace StudentManagement.Desktop.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
    }

    // DI-friendly constructor
    public DashboardView(DashboardViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}
