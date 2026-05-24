using System.Windows;
using StudentManagement.Desktop.ViewModels;

namespace StudentManagement.Desktop.Views;

public partial class LoginView : Window
{
    public LoginView(LoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
