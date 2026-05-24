using System.Windows;

namespace StudentManagement.Desktop.Views;

public partial class ConfirmationWindow : Window
{
    public bool UserConfirmed { get; private set; }
    public bool DoNotAskAgain => ChkDoNotAskAgain.IsChecked == true;

    public ConfirmationWindow(string message)
    {
        InitializeComponent();
        TxtMessage.Text = message;
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        UserConfirmed = true;
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        UserConfirmed = false;
        DialogResult = false;
        Close();
    }
}
