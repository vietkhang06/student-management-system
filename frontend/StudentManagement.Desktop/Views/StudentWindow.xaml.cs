using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using StudentManagement.Shared.Dtos.HocSinh;
using StudentManagement.Shared.Dtos.Lop;

namespace StudentManagement.Desktop.Views;

public partial class StudentWindow : Window
{
    public HocSinhCreateRequest? CreateResult { get; private set; }
    public HocSinhUpdateRequest? UpdateResult { get; private set; }

    private readonly HocSinhResponse? _existingStudent;

    public StudentWindow(List<LopResponse> classes, HocSinhResponse? existingStudent = null)
    {
        InitializeComponent();
        _existingStudent = existingStudent;

        CboClass.ItemsSource = classes;

        if (existingStudent != null)
        {
            // Edit mode
            TxtTitle.Text = "Chỉnh sửa học sinh";
            TxtIdHocSinh.Text = existingStudent.IdHocSinh;
            TxtIdHocSinh.IsEnabled = false;
            TxtTen.Text = existingStudent.Ten;
            TxtDiaChi.Text = existingStudent.DiaChi;
            TxtEmail.Text = existingStudent.Email;
            DpkNgaySinh.SelectedDate = existingStudent.NgaySinh;

            // Gender
            foreach (ComboBoxItem item in CboGender.Items)
            {
                if (string.Equals(item.Content?.ToString(), existingStudent.GioiTinh, StringComparison.OrdinalIgnoreCase))
                {
                    CboGender.SelectedItem = item;
                    break;
                }
            }

            // Class
            CboClass.SelectedItem = classes.FirstOrDefault(c => c.IdLop == existingStudent.IdLop);
        }
        else
        {
            // Add mode: default gender
            CboGender.SelectedIndex = 0;
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        TxtError.Visibility = Visibility.Collapsed;

        // Validation: Name
        if (string.IsNullOrWhiteSpace(TxtTen.Text))
        {
            ShowError("Vui lòng nhập Họ và tên.");
            return;
        }

        // Validation: Class
        if (CboClass.SelectedItem == null)
        {
            ShowError("Vui lòng chọn Lớp học.");
            return;
        }

        // Validation: Date of birth
        if (!DpkNgaySinh.SelectedDate.HasValue)
        {
            ShowError("Vui lòng nhập Ngày sinh.");
            return;
        }

        // Validation: Email format
        var email = TxtEmail.Text.Trim();
        if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
        {
            ShowError("Địa chỉ email không hợp lệ.");
            return;
        }
        if (string.IsNullOrWhiteSpace(email))
        {
            ShowError("Vui lòng nhập Email.");
            return;
        }

        var selectedClass = (LopResponse)CboClass.SelectedItem;
        var gender = (CboGender.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Nam";

        if (_existingStudent == null)
        {
            // Create mode
            CreateResult = new HocSinhCreateRequest
            {
                IdHocSinh = TxtIdHocSinh.Text.Trim(),
                Ten = TxtTen.Text.Trim(),
                GioiTinh = gender,
                NgaySinh = DpkNgaySinh.SelectedDate,
                DiaChi = TxtDiaChi.Text.Trim(),
                Email = email,
                IdLop = selectedClass.IdLop
            };
        }
        else
        {
            // Update mode
            UpdateResult = new HocSinhUpdateRequest
            {
                Ten = TxtTen.Text.Trim(),
                GioiTinh = gender,
                NgaySinh = DpkNgaySinh.SelectedDate,
                DiaChi = TxtDiaChi.Text.Trim(),
                Email = email,
                IdLop = selectedClass.IdLop
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

    private void ShowError(string message)
    {
        TxtError.Text = message;
        TxtError.Visibility = Visibility.Visible;
    }

    private static bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
    }
}
