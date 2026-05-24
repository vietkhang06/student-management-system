using System.Windows.Controls;
using StudentManagement.Desktop.ViewModels;

namespace StudentManagement.Desktop.Views;

public partial class ClassDetailView : UserControl
{
    public ClassDetailView()
    {
        InitializeComponent();
    }

    public ClassDetailView(ClassDetailViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}
