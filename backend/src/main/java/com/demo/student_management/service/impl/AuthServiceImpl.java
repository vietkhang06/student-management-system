package com.demo.student_management.service.impl;

import com.demo.student_management.dto.auth.AuthResponse;
import com.demo.student_management.dto.auth.LoginRequest;
import com.demo.student_management.dto.auth.RegisterRequest;
import com.demo.student_management.entity.TaiKhoan;
import com.demo.student_management.dto.auth.ProfileResponse;
import com.demo.student_management.repository.GiaoVienRepository;
import com.demo.student_management.repository.PhanCongGiangDayRepository;
import com.demo.student_management.repository.TaiKhoanRepository;
import com.demo.student_management.security.AuthTokenService;
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
    private final AuthTokenService authTokenService;
    private final GiaoVienRepository giaoVienRepository;
    private final PhanCongGiangDayRepository phanCongGiangDayRepository;

    @Override
    public AuthResponse register(RegisterRequest request) {
        if (taiKhoanRepository.existsByTenDangNhap(request.getTenDangNhap())) {
            throw new RuntimeException("Ten dang nhap da ton tai");
        }

        if (!request.getMatKhau().equals(request.getXacNhanMatKhau())) {
            throw new RuntimeException("Xac nhan mat khau khong khop");
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
                "Dang ky thanh cong",
                tk.getIdTaiKhoan(),
                tk.getTenDangNhap(),
                tk.getLoaiTaiKhoan(),
                authTokenService.createToken(tk)
        );
    }

    @Override
    public AuthResponse login(LoginRequest request) {
        TaiKhoan tk = taiKhoanRepository.findByTenDangNhap(request.getTenDangNhap())
                .orElseThrow(() -> new RuntimeException("Sai ten dang nhap hoac mat khau"));

        if (!passwordEncoder.matches(request.getMatKhau(), tk.getMatKhau())) {
            throw new RuntimeException("Sai ten dang nhap hoac mat khau");
        }

        return new AuthResponse(
                "Dang nhap thanh cong",
                tk.getIdTaiKhoan(),
                tk.getTenDangNhap(),
                tk.getLoaiTaiKhoan(),
                authTokenService.createToken(tk)
        );
    }

    @Override
    public ProfileResponse getProfile(String idTaiKhoan) {
        TaiKhoan tk = taiKhoanRepository.findById(idTaiKhoan)
                .orElseThrow(() -> new RuntimeException("Khong tim thay tai khoan"));

        String chuNhiemLopId = null;
        java.util.List<String> phanCongKeys = new java.util.ArrayList<>();

        if ("GIAOVIEN".equalsIgnoreCase(tk.getLoaiTaiKhoan())) {
            java.util.Optional<com.demo.student_management.entity.GiaoVien> gvOpt = giaoVienRepository.findByTaiKhoan_IdTaiKhoan(idTaiKhoan);
            if (gvOpt.isPresent()) {
                com.demo.student_management.entity.GiaoVien gv = gvOpt.get();
                if (gv.getLop() != null) {
                    chuNhiemLopId = gv.getLop().getIdLop();
                }
                java.util.List<com.demo.student_management.entity.PhanCongGiangDay> pcList = phanCongGiangDayRepository.findByGiaoVien_TaiKhoan_IdTaiKhoan(idTaiKhoan);
                for (com.demo.student_management.entity.PhanCongGiangDay pc : pcList) {
                    phanCongKeys.add(pc.getLop().getIdLop() + "_" + pc.getMonHoc().getIdMonHoc() + "_" + pc.getHocKy().getIdHocKy());
                }
            }
        }

        return ProfileResponse.builder()
                .idTaiKhoan(tk.getIdTaiKhoan())
                .tenDangNhap(tk.getTenDangNhap())
                .loaiTaiKhoan(tk.getLoaiTaiKhoan())
                .ten(tk.getTen())
                .cmnd(tk.getCmnd())
                .sdt(tk.getSdt())
                .ngaySinh(tk.getNgaySinh() != null ? tk.getNgaySinh().toString() : null)
                .gioiTinh(tk.getGioiTinh())
                .chuNhiemLopId(chuNhiemLopId)
                .phanCongKeys(phanCongKeys)
                .build();
    }
}
