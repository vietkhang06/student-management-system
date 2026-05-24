package com.demo.student_management.dto.auth;

import lombok.Data;

@Data
public class LoginRequest {
    private String tenDangNhap;
    private String matKhau;
}