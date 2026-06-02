using System.Windows;
using System.Windows.Controls;

namespace StudentManagement.Desktop.Views;

public partial class ReportView : UserControl
{
    public ReportView()
    {
        InitializeComponent();
    }

    private void TabSubjectClick(object sender, RoutedEventArgs e)
    {
        // Activate subject tab UI
        BtnTabSubject.BorderThickness  = new Thickness(0, 0, 0, 2);
        BtnTabSubject.BorderBrush      = System.Windows.Media.Brushes.Transparent;
        BtnTabSubject.Foreground       = new System.Windows.Media.SolidColorBrush(
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#4F46E5"));
        BtnTabSemester.BorderThickness = new Thickness(0);
        BtnTabSemester.Foreground      = new System.Windows.Media.SolidColorBrush(
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#64748B"));

        SubjectReportPanel.Visibility  = Visibility.Visible;
        SemesterReportPanel.Visibility = Visibility.Collapsed;
        BtnLoadSubject.Visibility      = Visibility.Visible;
        BtnLoadSemester.Visibility     = Visibility.Collapsed;
        SubjectFilterPanel.Visibility  = Visibility.Visible;
        SubjectFilterCol.Width         = new GridLength(1, GridUnitType.Star);
    }

    private void TabSemesterClick(object sender, RoutedEventArgs e)
    {
        // Activate semester tab UI
        BtnTabSemester.BorderThickness = new Thickness(0, 0, 0, 2);
        BtnTabSemester.BorderBrush     = System.Windows.Media.Brushes.Transparent;
        BtnTabSemester.Foreground      = new System.Windows.Media.SolidColorBrush(
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#4F46E5"));
        BtnTabSubject.BorderThickness  = new Thickness(0);
        BtnTabSubject.Foreground       = new System.Windows.Media.SolidColorBrush(
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#64748B"));

        SubjectReportPanel.Visibility  = Visibility.Collapsed;
        SemesterReportPanel.Visibility = Visibility.Visible;
        BtnLoadSubject.Visibility      = Visibility.Collapsed;
        BtnLoadSemester.Visibility     = Visibility.Visible;
        SubjectFilterPanel.Visibility  = Visibility.Collapsed;
        SubjectFilterCol.Width         = new GridLength(0);
    }
}
