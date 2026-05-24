package com.demo.student_management.controller;

import com.demo.student_management.dto.auth.AuthResponse;
import com.demo.student_management.dto.auth.LoginRequest;
import com.demo.student_management.dto.auth.RegisterRequest;
import com.demo.student_management.service.AuthService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import com.demo.student_management.dto.auth.ProfileResponse;
import com.demo.student_management.security.AuthenticatedUser;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;

@RestController
@RequestMapping("/api/auth")
@RequiredArgsConstructor
public class AuthController {

    private final AuthService authService;

    @PostMapping("/register")
    public ResponseEntity<AuthResponse> register(@RequestBody RegisterRequest request) {
        return ResponseEntity.ok(authService.register(request));
    }

    @PostMapping("/login")
    public ResponseEntity<AuthResponse> login(@RequestBody LoginRequest request) {
        return ResponseEntity.ok(authService.login(request));
    }

    @GetMapping("/me")
    public ResponseEntity<ProfileResponse> getMe() {
        Authentication authentication = SecurityContextHolder.getContext().getAuthentication();
        if (authentication == null || !(authentication.getPrincipal() instanceof AuthenticatedUser user)) {
            return ResponseEntity.status(401).build();
        }
        return ResponseEntity.ok(authService.getProfile(user.idTaiKhoan()));
    }
}