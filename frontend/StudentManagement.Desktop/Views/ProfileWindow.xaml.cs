using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using StudentManagement.Shared.Dtos.Auth;

namespace StudentManagement.Desktop.Views;

public partial class ProfileWindow : Window
{
    public ProfileWindow(ProfileResponse profile)
    {
        InitializeComponent();
        PopulateProfile(profile);
    }

    private void PopulateProfile(ProfileResponse profile)
    {
        // Name and Initials
        string fullName = string.IsNullOrWhiteSpace(profile.Ten) ? "Người dùng" : profile.Ten;
        TxtFullName.Text = fullName;

        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 0)
        {
            string initials = parts.Length == 1 
                ? parts[0][0].ToString() 
                : parts[0][0].ToString() + parts[parts.Length - 1][0].ToString();
            TxtInitials.Text = initials.ToUpper();
        }
        else
        {
            TxtInitials.Text = "U";
        }

        // Details
        TxtUsername.Text = profile.TenDangNhap;
        TxtCmnd.Text = string.IsNullOrWhiteSpace(profile.Cmnd) ? "Chưa cập nhật" : profile.Cmnd;
        TxtSdt.Text = string.IsNullOrWhiteSpace(profile.Sdt) ? "Chưa cập nhật" : profile.Sdt;
        TxtGender.Text = string.IsNullOrWhiteSpace(profile.GioiTinh) ? "Chưa cập nhật" : profile.GioiTinh;

        // Birthday format
        if (!string.IsNullOrWhiteSpace(profile.NgaySinh) && DateTime.TryParse(profile.NgaySinh, out var dob))
        {
            TxtDob.Text = dob.ToString("dd/MM/yyyy");
        }
        else
        {
            TxtDob.Text = "Chưa cập nhật";
        }

        // Role & Badges
        if (string.Equals(profile.LoaiTaiKhoan, "BANQUANLY", StringComparison.OrdinalIgnoreCase))
        {
            TxtRole.Text = "Ban Quản Lý";
            BadgeRole.Background = new SolidColorBrush(Color.FromRgb(254, 243, 199)); // Amber 100
            TxtRole.Foreground = new SolidColorBrush(Color.FromRgb(146, 64, 14));  // Amber 800
            PanelHomeroom.Visibility = Visibility.Collapsed;
        }
        else
        {
            TxtRole.Text = "Giáo Viên";
            BadgeRole.Background = new SolidColorBrush(Color.FromRgb(240, 253, 244)); // Green 100
            TxtRole.Foreground = new SolidColorBrush(Color.FromRgb(21, 128, 61));  // Green 700
            
            if (!string.IsNullOrWhiteSpace(profile.ChuNhiemLopId))
            {
                PanelHomeroom.Visibility = Visibility.Visible;
                TxtHomeroom.Text = $"Chủ nhiệm lớp {profile.ChuNhiemLopId}";
            }
            else
            {
                PanelHomeroom.Visibility = Visibility.Visible;
                TxtHomeroom.Text = "Không làm chủ nhiệm";
            }
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
