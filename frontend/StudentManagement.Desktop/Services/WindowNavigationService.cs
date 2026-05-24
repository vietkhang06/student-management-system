using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using StudentManagement.Desktop.Views;

namespace StudentManagement.Desktop.Services;

public sealed class WindowNavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public WindowNavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void ShowLogin()
    {
        var loginView = _serviceProvider.GetRequiredService<LoginView>();
        Application.Current.MainWindow = loginView;
        loginView.Show();

        CloseWindowsExcept(loginView);
    }

    public void ShowShell()
    {
        var shellWindow = _serviceProvider.GetRequiredService<ShellWindow>();
        Application.Current.MainWindow = shellWindow;
        shellWindow.Show();

        CloseWindowsExcept(shellWindow);
    }

    private static void CloseWindowsExcept(Window activeWindow)
    {
        foreach (Window window in Application.Current.Windows.Cast<Window>().ToList())
        {
            if (!ReferenceEquals(window, activeWindow))
            {
                window.Close();
            }
        }
    }
}
