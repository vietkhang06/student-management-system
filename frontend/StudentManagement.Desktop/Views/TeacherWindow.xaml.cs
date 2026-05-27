using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StudentManagement.Shared.Dtos.Admin;
using StudentManagement.Shared.Dtos.Lop;
using StudentManagement.Shared.Dtos.MonHoc;

namespace StudentManagement.Desktop.Views;

public partial class TeacherWindow : Window
{
    public TeacherCreateRequest? CreateResult { get; private set; }
    public TeacherUpdateRequest? UpdateResult { get; private set; }
    private readonly GiaoVienResponse? _existingTeacher;

    public TeacherWindow(List<LopResponse> classes, List<MonHocResponse> subjects, GiaoVienResponse? existingTeacher = null)
    {
        InitializeComponent();
        _existingTeacher = existingTeacher;

        CboClass.ItemsSource = classes;
        CboSubject.ItemsSource = subjects;

        if (existingTeacher != null)
        {
            TxtTitle.Text = "Chỉnh sửa tài khoản giáo viên";
            TxtIdGiaoVien.Text = existingTeacher.IdGiaoVien;
            TxtIdGiaoVien.IsEnabled = false;

            TxtUsername.Text = existingTeacher.TenDangNhap;
            TxtUsername.IsEnabled = false;

            TxtFullName.Text = existingTeacher.TenGiaoVien;
            
            LblPassword.Text = "Mật khẩu mới (để trống nếu không đổi)";
            TxtPassword.Text = string.Empty;

            // Set Gender ComboBox
            foreach (ComboBoxItem item in CboGender.Items)
            {
                if (string.Equals(item.Content?.ToString(), existingTeacher.GioiTinh, StringComparison.OrdinalIgnoreCase))
                {
                    CboGender.SelectedItem = item;
                    break;
                }
            }

            TxtSdt.Text = existingTeacher.Sdt;
            TxtEmail.Text = existingTeacher.Email;
            ChkActive.IsChecked = existingTeacher.Active;

            // Set Class ComboBox
            if (!string.IsNullOrEmpty(existingTeacher.IdLop))
            {
                CboClass.SelectedItem = classes.FirstOrDefault(c => c.IdLop == existingTeacher.IdLop);
            }

            // Set Subject ComboBox
            if (!string.IsNullOrEmpty(existingTeacher.IdMonHoc))
            {
                CboSubject.SelectedItem = subjects.FirstOrDefault(s => s.IdMonHoc == existingTeacher.IdMonHoc);
            }
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        // Validation
        if (_existingTeacher == null)
        {
            if (string.IsNullOrWhiteSpace(TxtIdGiaoVien.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã giáo viên", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(TxtUsername.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên đăng nhập", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(TxtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập Mật khẩu", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        if (string.IsNullOrWhiteSpace(TxtFullName.Text))
        {
            MessageBox.Show("Vui lòng nhập Họ và tên", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (CboClass.SelectedItem == null)
        {
            MessageBox.Show("Vui lòng chọn Lớp công tác/Chủ nhiệm", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var selectedClass = (LopResponse)CboClass.SelectedItem;
        var selectedSubject = (MonHocResponse?)CboSubject.SelectedItem;
        var gender = (CboGender.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;

        if (_existingTeacher == null)
        {
            CreateResult = new TeacherCreateRequest
            {
                IdGiaoVien = TxtIdGiaoVien.Text.Trim(),
                Ten = TxtFullName.Text.Trim(),
                TenDangNhap = TxtUsername.Text.Trim(),
                MatKhau = TxtPassword.Text,
                GioiTinh = gender,
                Sdt = TxtSdt.Text.Trim(),
                Email = TxtEmail.Text.Trim(),
                IdLop = selectedClass.IdLop,
                IdMonHoc = selectedSubject?.IdMonHoc ?? string.Empty,
                Active = ChkActive.IsChecked == true
            };
        }
        else
        {
            UpdateResult = new TeacherUpdateRequest
            {
                Ten = TxtFullName.Text.Trim(),
                MatKhau = TxtPassword.Text,
                GioiTinh = gender,
                Sdt = TxtSdt.Text.Trim(),
                Email = TxtEmail.Text.Trim(),
                IdLop = selectedClass.IdLop,
                IdMonHoc = selectedSubject?.IdMonHoc ?? string.Empty,
                Active = ChkActive.IsChecked == true
            };
        }

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
