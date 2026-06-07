using System.Windows.Controls;
using StudentManagement.Desktop.ViewModels;

namespace StudentManagement.Desktop.Views;

public partial class HistoryView : UserControl
{
    public HistoryView()
    {
        InitializeComponent();
    }

    public HistoryView(HistoryViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}
