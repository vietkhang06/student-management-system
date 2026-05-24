package com.demo.student_management.config;

import com.demo.student_management.entity.BanQuanLy;
import com.demo.student_management.entity.GiaoVien;
import com.demo.student_management.entity.Lop;
import com.demo.student_management.entity.TaiKhoan;
import com.demo.student_management.repository.BanQuanLyRepository;
import com.demo.student_management.repository.GiaoVienRepository;
import com.demo.student_management.repository.LopRepository;
import com.demo.student_management.repository.TaiKhoanRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.boot.CommandLineRunner;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

import java.util.UUID;

@Component
@RequiredArgsConstructor
public class DataInitializer implements CommandLineRunner {

    private final TaiKhoanRepository taiKhoanRepository;
    private final GiaoVienRepository giaoVienRepository;
    private final BanQuanLyRepository banQuanLyRepository;
    private final LopRepository lopRepository;
    private final PasswordEncoder passwordEncoder;

    @Override
    @Transactional
    public void run(String... args) {
        seedLop("L001", "10A1");
        seedLop("L002", "10A2");

        seedAdmin("admin01", "Admin@123", "Nguyen Van Quan");
        seedTeacher("gv01", "Gv@123456", "Le Thi Mai", "L001", "GV001");
        seedTeacher("gv02", "Gv@123456", "Tran Van Nam", "L002", "GV002");
    }

    private void seedLop(String idLop, String tenLop) {
        lopRepository.findById(idLop).orElseGet(() ->
                lopRepository.save(
                        Lop.builder()
                                .idLop(idLop)
                                .tenLop(tenLop)
                                .siSo(0)
                                .build()
                )
        );
    }

    private void seedAdmin(String username, String rawPassword, String fullName) {
        TaiKhoan tk = taiKhoanRepository.findByTenDangNhap(username)
                .orElseGet(() -> {
                    TaiKhoan newTk = TaiKhoan.builder()
                            .idTaiKhoan(UUID.randomUUID().toString())
                            .tenDangNhap(username)
                            .matKhau(passwordEncoder.encode(rawPassword))
                            .hash(passwordEncoder.encode(rawPassword))
                            .loaiTaiKhoan("BANQUANLY")
                            .ten(fullName)
                            .build();
                    return taiKhoanRepository.save(newTk);
                });

        if (banQuanLyRepository.findByTaiKhoan_IdTaiKhoan(tk.getIdTaiKhoan()).isEmpty()) {
            BanQuanLy bql = BanQuanLy.builder()
                    .idBanQl("BQL001")
                    .taiKhoan(tk)
                    .build();
            banQuanLyRepository.save(bql);
        }
    }

    private void seedTeacher(String username, String rawPassword, String fullName, String idLop, String idGiaoVien) {
        Lop lop = lopRepository.findById(idLop)
                .orElseThrow(() -> new RuntimeException("Thiếu lớp " + idLop + " để seed giáo viên"));

        TaiKhoan tk = taiKhoanRepository.findByTenDangNhap(username)
                .orElseGet(() -> {
                    TaiKhoan newTk = TaiKhoan.builder()
                            .idTaiKhoan(UUID.randomUUID().toString())
                            .tenDangNhap(username)
                            .matKhau(passwordEncoder.encode(rawPassword))
                            .hash(passwordEncoder.encode(rawPassword))
                            .loaiTaiKhoan("GIAOVIEN")
                            .ten(fullName)
                            .build();
                    return taiKhoanRepository.save(newTk);
                });

        if (giaoVienRepository.findByTaiKhoan_IdTaiKhoan(tk.getIdTaiKhoan()).isEmpty()) {
            GiaoVien gv = GiaoVien.builder()
                    .idGiaoVien(idGiaoVien)
                    .lop(lop)
                    .taiKhoan(tk)
                    .luong(0)
                    .build();
            giaoVienRepository.save(gv);
        }
    }
}