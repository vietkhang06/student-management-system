using System.Windows.Controls;
using StudentManagement.Desktop.ViewModels;

namespace StudentManagement.Desktop.Views;

public partial class ClassListView : UserControl
{
    public ClassListView()
    {
        InitializeComponent();
    }

    public ClassListView(ClassListViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}
