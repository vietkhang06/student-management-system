package com.demo.student_management.controller;

import com.demo.student_management.dto.admin.*;
import com.demo.student_management.entity.*;
import com.demo.student_management.repository.*;
import com.demo.student_management.security.AuthorizationService;
import com.demo.student_management.exception.BusinessException;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.UUID;

@RestController
@RequestMapping("/api/admin")
@RequiredArgsConstructor
public class SystemAdminController {

    private final HocSinhRepository hocSinhRepository;
    private final LopRepository lopRepository;
    private final GiaoVienRepository giaoVienRepository;
    private final MonHocRepository monHocRepository;
    private final HocKyRepository hocKyRepository;
    private final PhanCongGiangDayRepository phanCongGiangDayRepository;
    private final AuthorizationService authorizationService;

    @GetMapping("/overview")
    public ResponseEntity<SystemOverviewResponse> getOverview() {
        authorizationService.requireAdmin();

        long totalStudents = hocSinhRepository.count();
        long totalClasses = lopRepository.count();
        long totalTeachers = giaoVienRepository.count();
        long totalSubjects = monHocRepository.count();

        List<PhanCongResponse> assignments = phanCongGiangDayRepository.findAll().stream()
                .map(this::mapToPhanCongResponse)
                .toList();

        List<GiaoVienResponse> teachers = giaoVienRepository.findAll().stream()
                .map(gv -> GiaoVienResponse.builder()
                        .idGiaoVien(gv.getIdGiaoVien())
                        .tenGiaoVien(gv.getTaiKhoan().getTen() != null ? gv.getTaiKhoan().getTen() : gv.getTaiKhoan().getTenDangNhap())
                        .build())
                .toList();

        return ResponseEntity.ok(SystemOverviewResponse.builder()
                .totalStudents(totalStudents)
                .totalClasses(totalClasses)
                .totalTeachers(totalTeachers)
                .totalSubjects(totalSubjects)
                .assignments(assignments)
                .teachers(teachers)
                .build());
    }

    @PostMapping("/assignment")
    public ResponseEntity<PhanCongResponse> addAssignment(@RequestBody PhanCongRequest request) {
        authorizationService.requireAdmin();

        GiaoVien gv = giaoVienRepository.findById(request.getIdGiaoVien())
                .orElseThrow(() -> new BusinessException("Khong tim thay giao vien"));
        Lop lop = lopRepository.findById(request.getIdLop())
                .orElseThrow(() -> new BusinessException("Khong tim thay lop"));
        MonHoc mon = monHocRepository.findById(request.getIdMonHoc())
                .orElseThrow(() -> new BusinessException("Khong tim thay mon hoc"));
        HocKy hk = hocKyRepository.findById(request.getIdHocKy())
                .orElseThrow(() -> new BusinessException("Khong tim thay hoc ky"));

        // Check if exists
        boolean exists = phanCongGiangDayRepository.existsByGiaoVien_TaiKhoan_IdTaiKhoanAndLop_IdLopAndMonHoc_IdMonHocAndHocKy_IdHocKy(
                gv.getTaiKhoan().getIdTaiKhoan(), lop.getIdLop(), mon.getIdMonHoc(), hk.getIdHocKy()
        );
        if (exists) {
            throw new BusinessException("Phan cong giang day da ton tai");
        }

        PhanCongGiangDay pc = PhanCongGiangDay.builder()
                .idPhanCong(UUID.randomUUID().toString())
                .giaoVien(gv)
                .lop(lop)
                .monHoc(mon)
                .hocKy(hk)
                .build();

        PhanCongGiangDay saved = phanCongGiangDayRepository.save(pc);
        return ResponseEntity.ok(mapToPhanCongResponse(saved));
    }

    @DeleteMapping("/assignment/{idPhanCong}")
    public ResponseEntity<Void> deleteAssignment(@PathVariable String idPhanCong) {
        authorizationService.requireAdmin();

        if (!phanCongGiangDayRepository.existsById(idPhanCong)) {
            throw new BusinessException("Khong tim thay phan cong giang day");
        }

        phanCongGiangDayRepository.deleteById(idPhanCong);
        return ResponseEntity.ok().build();
    }

    private PhanCongResponse mapToPhanCongResponse(PhanCongGiangDay pc) {
        return PhanCongResponse.builder()
                .idPhanCong(pc.getIdPhanCong())
                .idGiaoVien(pc.getGiaoVien().getIdGiaoVien())
                .tenGiaoVien(pc.getGiaoVien().getTaiKhoan().getTen() != null ? pc.getGiaoVien().getTaiKhoan().getTen() : pc.getGiaoVien().getTaiKhoan().getTenDangNhap())
                .idLop(pc.getLop().getIdLop())
                .tenLop(pc.getLop().getTenLop())
                .idMonHoc(pc.getMonHoc().getIdMonHoc())
                .tenMonHoc(pc.getMonHoc().getTenMonHoc())
                .idHocKy(pc.getHocKy().getIdHocKy())
                .tenHocKy(pc.getHocKy().getTenHocKy() + " — " + pc.getHocKy().getNamHoc())
                .build();
    }
}
