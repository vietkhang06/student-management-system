using System;
using System.Windows;
using StudentManagement.Shared.Dtos.Admin;
using StudentManagement.Shared.Dtos.MonHoc;

namespace StudentManagement.Desktop.Views;

public partial class SubjectWindow : Window
{
    public SubjectRequest? Result { get; private set; }
    private readonly MonHocResponse? _existingSubject;

    public SubjectWindow(MonHocResponse? existingSubject = null)
    {
        InitializeComponent();
        _existingSubject = existingSubject;

        if (existingSubject != null)
        {
            TxtTitle.Text = "Chỉnh sửa môn học";
            TxtIdMonHoc.Text = existingSubject.IdMonHoc;
            TxtIdMonHoc.IsEnabled = false;

            TxtTenMonHoc.Text = existingSubject.TenMonHoc;
            ChkActive.IsChecked = existingSubject.TrangThaiSuDung;
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (_existingSubject == null)
        {
            if (string.IsNullOrWhiteSpace(TxtIdMonHoc.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã môn học", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        if (string.IsNullOrWhiteSpace(TxtTenMonHoc.Text))
        {
            MessageBox.Show("Vui lòng nhập Tên môn học", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Result = new SubjectRequest
        {
            IdMonHoc = TxtIdMonHoc.Text.Trim(),
            TenMonHoc = TxtTenMonHoc.Text.Trim(),
            TrangThaiSuDung = ChkActive.IsChecked == true
        };

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
