using StudentManagement.Desktop.ViewModels;
using System.Windows;

namespace StudentManagement.Desktop.Views;

public partial class ShellWindow : Window
{
    public ShellWindow(ShellViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}