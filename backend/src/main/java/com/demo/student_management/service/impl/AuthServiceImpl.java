package com.demo.student_management.service.impl;

import com.demo.student_management.dto.auth.AuthResponse;
import com.demo.student_management.dto.auth.LoginRequest;
import com.demo.student_management.dto.auth.RegisterRequest;
import com.demo.student_management.entity.TaiKhoan;
import com.demo.student_management.repository.TaiKhoanRepository;
import com.demo.student_management.service.AuthService;
import lombok.RequiredArgsConstructor;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;

import java.time.LocalDate;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class AuthServiceImpl implements AuthService {

    private final TaiKhoanRepository taiKhoanRepository;
    private final PasswordEncoder passwordEncoder;

    @Override
    public AuthResponse register(RegisterRequest request) {
        if (taiKhoanRepository.existsByTenDangNhap(request.getTenDangNhap())) {
            throw new RuntimeException("Tên đăng nhập đã tồn tại");
        }

        if (!request.getMatKhau().equals(request.getXacNhanMatKhau())) {
            throw new RuntimeException("Xác nhận mật khẩu không khớp");
        }

        TaiKhoan tk = new TaiKhoan();
        tk.setIdTaiKhoan(UUID.randomUUID().toString());
        tk.setTenDangNhap(request.getTenDangNhap());
        tk.setMatKhau(passwordEncoder.encode(request.getMatKhau()));
        tk.setLoaiTaiKhoan(request.getLoaiTaiKhoan());
        tk.setTen(request.getTen());
        tk.setCmnd(request.getCmnd());
        tk.setGioiTinh(request.getGioiTinh());
        tk.setSdt(request.getSdt());

        if (request.getNgaySinh() != null && !request.getNgaySinh().isBlank()) {
            tk.setNgaySinh(LocalDate.parse(request.getNgaySinh()));
        }

        taiKhoanRepository.save(tk);

        return new AuthResponse(
                "Đăng ký thành công",
                tk.getIdTaiKhoan(),
                tk.getTenDangNhap(),
                tk.getLoaiTaiKhoan()
        );
    }

    @Override
    public AuthResponse login(LoginRequest request) {
        TaiKhoan tk = taiKhoanRepository.findByTenDangNhap(request.getTenDangNhap())
                .orElseThrow(() -> new RuntimeException("Sai tên đăng nhập hoặc mật khẩu"));

        if (!passwordEncoder.matches(request.getMatKhau(), tk.getMatKhau())) {
            throw new RuntimeException("Sai tên đăng nhập hoặc mật khẩu");
        }

        return new AuthResponse(
                "Đăng nhập thành công",
                tk.getIdTaiKhoan(),
                tk.getTenDangNhap(),
                tk.getLoaiTaiKhoan()
        );
    }
}