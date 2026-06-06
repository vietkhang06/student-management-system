using StudentManagement.Desktop.ViewModels;
using System.Windows;
using System.Windows.Media.Animation;

namespace StudentManagement.Desktop.Views;

public partial class ShellWindow : Window
{
    public ShellWindow(ShellViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void Sidebar_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        var anim = (Storyboard)this.Resources["OpenMenuAnimation"];
        anim.Begin();
    }

    private void Sidebar_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        var anim = (Storyboard)this.Resources["CloseMenuAnimation"];
        anim.Begin();
    }
}