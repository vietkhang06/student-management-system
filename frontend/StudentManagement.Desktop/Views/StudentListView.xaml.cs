using System.Windows.Controls;
using StudentManagement.Desktop.ViewModels;

namespace StudentManagement.Desktop.Views;

public partial class StudentListView : UserControl
{
    public StudentListView()
    {
        InitializeComponent();
    }

    public StudentListView(StudentListViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}
