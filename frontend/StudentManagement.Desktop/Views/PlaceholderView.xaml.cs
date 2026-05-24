using System.Windows.Controls;
using StudentManagement.Desktop.ViewModels;

namespace StudentManagement.Desktop.Views;

public partial class PlaceholderView : UserControl
{
    public PlaceholderView()
    {
        InitializeComponent();
    }

    public PlaceholderView(PlaceholderViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}
