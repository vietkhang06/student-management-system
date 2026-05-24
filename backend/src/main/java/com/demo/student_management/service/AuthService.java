package com.demo.student_management.service;

import com.demo.student_management.dto.auth.AuthResponse;
import com.demo.student_management.dto.auth.LoginRequest;
import com.demo.student_management.dto.auth.RegisterRequest;

public interface AuthService {
    AuthResponse register(RegisterRequest request);
    AuthResponse login(LoginRequest request);
    com.demo.student_management.dto.auth.ProfileResponse getProfile(String idTaiKhoan);
}