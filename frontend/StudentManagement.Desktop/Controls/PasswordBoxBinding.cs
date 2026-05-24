using System.Windows;
using System.Windows.Controls;

namespace StudentManagement.Desktop.Controls;

public static class PasswordBoxBinding
{
    public static readonly DependencyProperty BoundPasswordProperty =
        DependencyProperty.RegisterAttached(
            "BoundPassword",
            typeof(string),
            typeof(PasswordBoxBinding),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundPasswordChanged));

    private static readonly DependencyProperty UpdatingPasswordProperty =
        DependencyProperty.RegisterAttached(
            "UpdatingPassword",
            typeof(bool),
            typeof(PasswordBoxBinding));

    public static string GetBoundPassword(DependencyObject dependencyObject)
    {
        return (string)dependencyObject.GetValue(BoundPasswordProperty);
    }

    public static void SetBoundPassword(DependencyObject dependencyObject, string value)
    {
        dependencyObject.SetValue(BoundPasswordProperty, value);
    }

    private static bool GetUpdatingPassword(DependencyObject dependencyObject)
    {
        return (bool)dependencyObject.GetValue(UpdatingPasswordProperty);
    }

    private static void SetUpdatingPassword(DependencyObject dependencyObject, bool value)
    {
        dependencyObject.SetValue(UpdatingPasswordProperty, value);
    }

    private static void OnBoundPasswordChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not PasswordBox passwordBox)
        {
            return;
        }

        passwordBox.PasswordChanged -= OnPasswordChanged;

        if (!GetUpdatingPassword(passwordBox))
        {
            passwordBox.Password = e.NewValue?.ToString() ?? string.Empty;
        }

        passwordBox.PasswordChanged += OnPasswordChanged;
    }

    private static void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not PasswordBox passwordBox)
        {
            return;
        }

        SetUpdatingPassword(passwordBox, true);
        SetBoundPassword(passwordBox, passwordBox.Password);
        SetUpdatingPassword(passwordBox, false);
    }
}
