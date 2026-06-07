package com.demo.student_management.controller;

import com.demo.student_management.dto.admin.*;
import com.demo.student_management.dto.monhoc.MonHocResponse;
import com.demo.student_management.entity.*;
import com.demo.student_management.repository.*;
import com.demo.student_management.security.AuthorizationService;
import com.demo.student_management.exception.BusinessException;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.AccessDeniedException;
import org.springframework.web.bind.annotation.*;

import com.demo.student_management.service.LichSuHeThongService;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@RestController
@RequestMapping("/api/admin")
@RequiredArgsConstructor
@org.springframework.transaction.annotation.Transactional
public class SystemAdminController {

    private final HocSinhRepository hocSinhRepository;
    private final LopRepository lopRepository;
    private final GiaoVienRepository giaoVienRepository;
    private final MonHocRepository monHocRepository;
    private final HocKyRepository hocKyRepository;
    private final PhanCongGiangDayRepository phanCongGiangDayRepository;
    private final AuthorizationService authorizationService;
    private final TaiKhoanRepository taiKhoanRepository;
    private final org.springframework.security.crypto.password.PasswordEncoder passwordEncoder;
    private final ChiTietDiemRepository chiTietDiemRepository;
    private final LichSuHeThongService lichSuHeThongService;

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
                .map(this::mapToGiaoVienResponse)
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

    @GetMapping("/health")
    public ResponseEntity<SystemHealthResponse> getHealth() {
        authorizationService.requireAdmin();

        String dbStatus = "UP";
        try {
            lopRepository.count();
        } catch (Exception e) {
            dbStatus = "DOWN: " + e.getMessage();
        }

        Runtime runtime = Runtime.getRuntime();
        long totalMemory = runtime.totalMemory();
        long freeMemory = runtime.freeMemory();
        long usedMemory = totalMemory - freeMemory;
        String jvmVersion = System.getProperty("java.version");
        String osName = System.getProperty("os.name");

        return ResponseEntity.ok(SystemHealthResponse.builder()
                .databaseStatus(dbStatus)
                .jvmVersion(jvmVersion)
                .totalMemoryBytes(totalMemory)
                .freeMemoryBytes(freeMemory)
                .usedMemoryBytes(usedMemory)
                .osName(osName)
                .build());
    }

    // ── TEACHER CRUD ──────────────────────────────────────────────────────────

    @GetMapping("/teachers")
    public ResponseEntity<List<GiaoVienResponse>> getTeachers() {
        authorizationService.requireAdmin();
        List<GiaoVienResponse> list = giaoVienRepository.findAll().stream()
                .map(this::mapToGiaoVienResponse)
                .toList();
        return ResponseEntity.ok(list);
    }

    @PostMapping("/teachers")
    @org.springframework.transaction.annotation.Transactional
    public ResponseEntity<GiaoVienResponse> createTeacher(@RequestBody TeacherCreateRequest request) {
        authorizationService.requireAdmin();

        if (request.getIdGiaoVien() == null || request.getIdGiaoVien().trim().isEmpty()) {
            throw new BusinessException("Mã giáo viên không được để trống");
        }
        if (request.getTenDangNhap() == null || request.getTenDangNhap().trim().isEmpty()) {
            throw new BusinessException("Tên đăng nhập không được để trống");
        }
        if (request.getMatKhau() == null || request.getMatKhau().trim().isEmpty()) {
            throw new BusinessException("Mật khẩu không được để trống");
        }

        if (giaoVienRepository.existsById(request.getIdGiaoVien().trim())) {
            throw new BusinessException("Mã giáo viên đã tồn tại");
        }
        if (taiKhoanRepository.existsByTenDangNhap(request.getTenDangNhap().trim())) {
            throw new BusinessException("Tên đăng nhập đã tồn tại");
        }

        Lop lop = lopRepository.findById(request.getIdLop())
                .orElseThrow(() -> new BusinessException("Không tìm thấy lớp học"));

        Lop lopChuNhiem = null;
        if (request.getIdLopChuNhiem() != null && !request.getIdLopChuNhiem().trim().isEmpty()) {
            lopChuNhiem = lopRepository.findById(request.getIdLopChuNhiem())
                    .orElseThrow(() -> new BusinessException("Không tìm thấy lớp chủ nhiệm"));
            
            Optional<GiaoVien> existingChuNhiem = giaoVienRepository.findByLopChuNhiem_IdLop(lopChuNhiem.getIdLop());
            if (existingChuNhiem.isPresent()) {
                throw new BusinessException("Lớp " + lopChuNhiem.getTenLop() + " đã có giáo viên chủ nhiệm khác: " + existingChuNhiem.get().getTaiKhoan().getTen());
            }
        }

        MonHoc monHoc = null;
        if (request.getIdMonHoc() != null && !request.getIdMonHoc().trim().isEmpty()) {
            monHoc = monHocRepository.findById(request.getIdMonHoc())
                    .orElseThrow(() -> new BusinessException("Không tìm thấy môn học"));
        }

        TaiKhoan tk = TaiKhoan.builder()
                .idTaiKhoan(UUID.randomUUID().toString())
                .tenDangNhap(request.getTenDangNhap().trim())
                .matKhau(passwordEncoder.encode(request.getMatKhau()))
                .hash(passwordEncoder.encode(request.getMatKhau()))
                .loaiTaiKhoan("GIAOVIEN")
                .ten(request.getTen())
                .gioiTinh(request.getGioiTinh())
                .email(request.getEmail())
                .sdt(request.getSdt())
                .active(request.getActive() != null ? request.getActive() : true)
                .build();
        tk = taiKhoanRepository.save(tk);

        GiaoVien gv = GiaoVien.builder()
                .idGiaoVien(request.getIdGiaoVien().trim())
                .taiKhoan(tk)
                .lop(lop)
                .lopChuNhiem(lopChuNhiem)
                .monHoc(monHoc)
                .luong(0)
                .build();
        gv = giaoVienRepository.save(gv);

        lichSuHeThongService.log("Thêm giáo viên", String.format("Thêm giáo viên [%s - %s] dạy môn [%s], chủ nhiệm lớp [%s]", 
                gv.getIdGiaoVien(), tk.getTen(), 
                gv.getMonHoc() != null ? gv.getMonHoc().getTenMonHoc() : "Không", 
                gv.getLopChuNhiem() != null ? gv.getLopChuNhiem().getTenLop() : "Không"));

        return ResponseEntity.ok(mapToGiaoVienResponse(gv));
    }

    @PutMapping("/teachers/{idGiaoVien}")
    @org.springframework.transaction.annotation.Transactional
    public ResponseEntity<GiaoVienResponse> updateTeacher(
            @PathVariable String idGiaoVien,
            @RequestBody TeacherUpdateRequest request) {
        authorizationService.requireAdmin();

        GiaoVien gv = giaoVienRepository.findById(idGiaoVien)
                .orElseThrow(() -> new BusinessException("Không tìm thấy giáo viên"));

        Lop lop = lopRepository.findById(request.getIdLop())
                .orElseThrow(() -> new BusinessException("Không tìm thấy lớp học"));

        Lop lopChuNhiem = null;
        if (request.getIdLopChuNhiem() != null && !request.getIdLopChuNhiem().trim().isEmpty()) {
            lopChuNhiem = lopRepository.findById(request.getIdLopChuNhiem())
                    .orElseThrow(() -> new BusinessException("Không tìm thấy lớp chủ nhiệm"));
            
            Optional<GiaoVien> existingChuNhiem = giaoVienRepository.findByLopChuNhiem_IdLop(lopChuNhiem.getIdLop());
            if (existingChuNhiem.isPresent() && !existingChuNhiem.get().getIdGiaoVien().equals(idGiaoVien)) {
                throw new BusinessException("Lớp " + lopChuNhiem.getTenLop() + " đã có giáo viên chủ nhiệm khác: " + existingChuNhiem.get().getTaiKhoan().getTen());
            }
        }

        MonHoc monHoc = null;
        if (request.getIdMonHoc() != null && !request.getIdMonHoc().trim().isEmpty()) {
            monHoc = monHocRepository.findById(request.getIdMonHoc())
                    .orElseThrow(() -> new BusinessException("Không tìm thấy môn học"));
        }

        TaiKhoan tk = gv.getTaiKhoan();
        tk.setTen(request.getTen());
        tk.setGioiTinh(request.getGioiTinh());
        tk.setEmail(request.getEmail());
        tk.setSdt(request.getSdt());
        if (request.getActive() != null) {
            tk.setActive(request.getActive());
        }

        if (request.getMatKhau() != null && !request.getMatKhau().trim().isEmpty()) {
            tk.setMatKhau(passwordEncoder.encode(request.getMatKhau()));
            tk.setHash(passwordEncoder.encode(request.getMatKhau()));
        }

        taiKhoanRepository.save(tk);

        gv.setLop(lop);
        gv.setLopChuNhiem(lopChuNhiem);
        gv.setMonHoc(monHoc);
        gv = giaoVienRepository.save(gv);

        lichSuHeThongService.log("Sửa giáo viên", String.format("Sửa thông tin giáo viên [%s - %s]. Dạy môn: %s, Chủ nhiệm: %s", 
                gv.getIdGiaoVien(), tk.getTen(), 
                gv.getMonHoc() != null ? gv.getMonHoc().getTenMonHoc() : "Không", 
                gv.getLopChuNhiem() != null ? gv.getLopChuNhiem().getTenLop() : "Không"));

        return ResponseEntity.ok(mapToGiaoVienResponse(gv));
    }

    @DeleteMapping("/teachers/{idGiaoVien}")
    @org.springframework.transaction.annotation.Transactional
    public ResponseEntity<Void> deleteTeacher(@PathVariable String idGiaoVien) {
        authorizationService.requireAdmin();

        GiaoVien gv = giaoVienRepository.findById(idGiaoVien)
                .orElseThrow(() -> new BusinessException("Không tìm thấy giáo viên"));

        com.demo.student_management.security.AuthenticatedUser currentUser = authorizationService.currentUser()
                .orElseThrow(() -> new AccessDeniedException("Chưa đăng nhập"));

        if (gv.getTaiKhoan().getIdTaiKhoan().equals(currentUser.idTaiKhoan())) {
            throw new BusinessException("Không thể tự xóa tài khoản đang đăng nhập");
        }

        // Delete teaching assignments first
        List<PhanCongGiangDay> pcList = phanCongGiangDayRepository.findByGiaoVien_IdGiaoVien(idGiaoVien);
        if (!pcList.isEmpty()) {
            phanCongGiangDayRepository.deleteAll(pcList);
        }

        // Now delete the teacher and account
        TaiKhoan tk = gv.getTaiKhoan();
        giaoVienRepository.delete(gv);
        taiKhoanRepository.delete(tk);

        lichSuHeThongService.log("Xóa giáo viên", String.format("Xóa giáo viên [%s - %s]", gv.getIdGiaoVien(), tk.getTen()));

        return ResponseEntity.ok().build();
    }

    // ── SUBJECT CRUD ──────────────────────────────────────────────────────────

    @GetMapping("/subjects")
    public ResponseEntity<List<MonHocResponse>> getSubjects() {
        authorizationService.requireAdmin();
        List<MonHocResponse> list = monHocRepository.findAll().stream()
                .map(m -> new MonHocResponse(m.getIdMonHoc(), m.getTenMonHoc(), m.getTrangThaiSuDung() != null ? m.getTrangThaiSuDung() : true))
                .toList();
        return ResponseEntity.ok(list);
    }

    @PostMapping("/subjects")
    public ResponseEntity<MonHocResponse> createSubject(@RequestBody SubjectRequest request) {
        authorizationService.requireAdmin();

        if (request.getIdMonHoc() == null || request.getIdMonHoc().trim().isEmpty()) {
            throw new BusinessException("Mã môn học không được để trống");
        }
        if (request.getTenMonHoc() == null || request.getTenMonHoc().trim().isEmpty()) {
            throw new BusinessException("Tên môn học không được để trống");
        }

        if (monHocRepository.existsById(request.getIdMonHoc().trim())) {
            throw new BusinessException("Mã môn học đã tồn tại");
        }

        MonHoc mh = MonHoc.builder()
                .idMonHoc(request.getIdMonHoc().trim())
                .tenMonHoc(request.getTenMonHoc().trim())
                .trangThaiSuDung(request.getTrangThaiSuDung() != null ? request.getTrangThaiSuDung() : true)
                .build();
        mh = monHocRepository.save(mh);

        return ResponseEntity.ok(new MonHocResponse(mh.getIdMonHoc(), mh.getTenMonHoc(), mh.getTrangThaiSuDung()));
    }

    @PutMapping("/subjects/{idMonHoc}")
    public ResponseEntity<MonHocResponse> updateSubject(
            @PathVariable String idMonHoc,
            @RequestBody SubjectRequest request) {
        authorizationService.requireAdmin();

        MonHoc mh = monHocRepository.findById(idMonHoc)
                .orElseThrow(() -> new BusinessException("Không tìm thấy môn học"));

        if (request.getTenMonHoc() == null || request.getTenMonHoc().trim().isEmpty()) {
            throw new BusinessException("Tên môn học không được để trống");
        }

        mh.setTenMonHoc(request.getTenMonHoc().trim());
        if (request.getTrangThaiSuDung() != null) {
            mh.setTrangThaiSuDung(request.getTrangThaiSuDung());
        }

        mh = monHocRepository.save(mh);
        return ResponseEntity.ok(new MonHocResponse(mh.getIdMonHoc(), mh.getTenMonHoc(), mh.getTrangThaiSuDung()));
    }

    @DeleteMapping("/subjects/{idMonHoc}")
    public ResponseEntity<Void> deleteSubject(@PathVariable String idMonHoc) {
        authorizationService.requireAdmin();

        if (!monHocRepository.existsById(idMonHoc)) {
            throw new BusinessException("Không tìm thấy môn học");
        }

        // Prevent deletion if used in ChiTietDiem or PhanCongGiangDay
        boolean usedInScores = chiTietDiemRepository.existsByMonHoc_IdMonHoc(idMonHoc);
        boolean usedInAssignments = phanCongGiangDayRepository.existsByMonHoc_IdMonHoc(idMonHoc);

        if (usedInScores || usedInAssignments) {
            throw new BusinessException("Môn học đang được sử dụng trong bảng điểm hoặc phân công giảng dạy, không thể xóa");
        }

        monHocRepository.deleteById(idMonHoc);
        return ResponseEntity.ok().build();
    }

    // ── ASSIGNMENTS ───────────────────────────────────────────────────────────

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
        boolean exists = phanCongGiangDayRepository.existsByLop_IdLopAndMonHoc_IdMonHocAndHocKy_IdHocKy(
                lop.getIdLop(), mon.getIdMonHoc(), hk.getIdHocKy()
        );
        if (exists) {
            throw new BusinessException("Môn học " + mon.getTenMonHoc() + " tại lớp " + lop.getTenLop() + " trong học kỳ đã chọn đã được phân công cho giáo viên khác.");
        }

        String newId = String.format("PC_%s_%s_%s_%s", gv.getIdGiaoVien(), lop.getIdLop(), mon.getIdMonHoc(), hk.getIdHocKy());

        PhanCongGiangDay pc = PhanCongGiangDay.builder()
                .idPhanCong(newId)
                .giaoVien(gv)
                .lop(lop)
                .monHoc(mon)
                .hocKy(hk)
                .build();

        PhanCongGiangDay saved = phanCongGiangDayRepository.save(pc);

        if (gv.getMonHoc() == null) {
            gv.setMonHoc(mon);
            giaoVienRepository.save(gv);
        }

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

    // ── HELPERS ───────────────────────────────────────────────────────────────

    private GiaoVienResponse mapToGiaoVienResponse(GiaoVien gv) {
        return GiaoVienResponse.builder()
                .idGiaoVien(gv.getIdGiaoVien())
                .tenGiaoVien(gv.getTaiKhoan().getTen() != null ? gv.getTaiKhoan().getTen() : gv.getTaiKhoan().getTenDangNhap())
                .tenDangNhap(gv.getTaiKhoan().getTenDangNhap())
                .sdt(gv.getTaiKhoan().getSdt() != null ? gv.getTaiKhoan().getSdt() : "")
                .email(gv.getTaiKhoan().getEmail() != null ? gv.getTaiKhoan().getEmail() : "")
                .gioiTinh(gv.getTaiKhoan().getGioiTinh() != null ? gv.getTaiKhoan().getGioiTinh() : "")
                .idLop(gv.getLop().getIdLop())
                .tenLop(gv.getLop().getTenLop())
                .idLopChuNhiem(gv.getLopChuNhiem() != null ? gv.getLopChuNhiem().getIdLop() : "")
                .tenLopChuNhiem(gv.getLopChuNhiem() != null ? gv.getLopChuNhiem().getTenLop() : "Không")
                .idMonHoc(gv.getMonHoc() != null ? gv.getMonHoc().getIdMonHoc() : "")
                .tenMonHoc(gv.getMonHoc() != null ? gv.getMonHoc().getTenMonHoc() : "Chưa phân công")
                .active(gv.getTaiKhoan().getActive() != null ? gv.getTaiKhoan().getActive() : true)
                .build();
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
