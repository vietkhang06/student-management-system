package com.demo.student_management.dto.auth;

import lombok.Data;

@Data
public class RegisterRequest {
    private String tenDangNhap;
    private String matKhau;
    private String xacNhanMatKhau;
    private String loaiTaiKhoan; // GIAOVIEN / BANQUANLY

    private String ten;
    private String cmnd;
    private String ngaySinh;     // tạm để String cho dễ test
    private String gioiTinh;
    private String sdt;
}