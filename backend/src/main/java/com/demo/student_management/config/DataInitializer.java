package com.demo.student_management.config;

import com.demo.student_management.entity.BanQuanLy;
import com.demo.student_management.entity.GiaoVien;
import com.demo.student_management.entity.Lop;
import com.demo.student_management.entity.TaiKhoan;
import com.demo.student_management.entity.MonHoc;
import com.demo.student_management.entity.HocKy;
import com.demo.student_management.entity.PhanCongGiangDay;
import com.demo.student_management.repository.BanQuanLyRepository;
import com.demo.student_management.repository.GiaoVienRepository;
import com.demo.student_management.repository.LopRepository;
import com.demo.student_management.repository.TaiKhoanRepository;
import com.demo.student_management.repository.MonHocRepository;
import com.demo.student_management.repository.HocKyRepository;
import com.demo.student_management.repository.PhanCongGiangDayRepository;
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
    private final MonHocRepository monHocRepository;
    private final HocKyRepository hocKyRepository;
    private final PhanCongGiangDayRepository phanCongGiangDayRepository;
    private final PasswordEncoder passwordEncoder;

    @Override
    @Transactional
    public void run(String... args) {
        seedLop("L001", "10A1");
        seedLop("L002", "10A2");

        seedAdmin("admin01", "Admin@123", "Nguyen Van Quan");
        seedTeacher("gv01", "Gv@123456", "Le Thi Mai", "L001", "GV001", "MH001");
        seedTeacher("gv02", "Gv@123456", "Tran Van Nam", "L002", "GV002", "MH002");

        // Seed teaching assignments: GV001 for L001, GV002 for L002 for all subjects & terms
        for (int i = 1; i <= 9; i++) {
            String idMonHoc = String.format("MH%03d", i);
            seedAssignment("GV001", "L001", idMonHoc, "HKI");
            seedAssignment("GV001", "L001", idMonHoc, "HKII");

            seedAssignment("GV002", "L002", idMonHoc, "HKI");
            seedAssignment("GV002", "L002", idMonHoc, "HKII");
        }
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
                            .active(true)
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

    private void seedTeacher(String username, String rawPassword, String fullName, String idLop, String idGiaoVien, String idMonHoc) {
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
                            .active(true)
                            .build();
                    return taiKhoanRepository.save(newTk);
                });

        if (giaoVienRepository.findByTaiKhoan_IdTaiKhoan(tk.getIdTaiKhoan()).isEmpty()) {
            MonHoc monHoc = monHocRepository.findById(idMonHoc).orElse(null);
            GiaoVien gv = GiaoVien.builder()
                    .idGiaoVien(idGiaoVien)
                    .lop(lop)
                    .taiKhoan(tk)
                    .monHoc(monHoc)
                    .luong(0)
                    .build();
            giaoVienRepository.save(gv);
        }
    }

    private void seedAssignment(String idGiaoVien, String idLop, String idMonHoc, String idHocKy) {
        String idPhanCong = idGiaoVien + "_" + idLop + "_" + idMonHoc + "_" + idHocKy;
        if (!phanCongGiangDayRepository.existsById(idPhanCong)) {
            GiaoVien gv = giaoVienRepository.findById(idGiaoVien)
                    .orElseThrow(() -> new RuntimeException("Thiếu giáo viên " + idGiaoVien + " để seed phân công"));
            Lop lop = lopRepository.findById(idLop)
                    .orElseThrow(() -> new RuntimeException("Thiếu lớp " + idLop + " để seed phân công"));
            MonHoc monHoc = monHocRepository.findById(idMonHoc)
                    .orElseThrow(() -> new RuntimeException("Thiếu môn học " + idMonHoc + " để seed phân công"));
            HocKy hocKy = hocKyRepository.findById(idHocKy)
                    .orElseThrow(() -> new RuntimeException("Thiếu học kỳ " + idHocKy + " để seed phân công"));

            phanCongGiangDayRepository.save(PhanCongGiangDay.builder()
                    .idPhanCong(idPhanCong)
                    .giaoVien(gv)
                    .lop(lop)
                    .monHoc(monHoc)
                    .hocKy(hocKy)
                    .build());
        }
    }
}