package com.demo.student_management.security;

public record AuthenticatedUser(
        String idTaiKhoan,
        String tenDangNhap,
        String loaiTaiKhoan
) {
}
