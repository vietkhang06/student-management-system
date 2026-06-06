package com.demo.student_management.security;

import com.demo.student_management.entity.GiaoVien;
import com.demo.student_management.entity.HocSinh;
import com.demo.student_management.entity.Lop;
import com.demo.student_management.entity.PhanCongGiangDay;
import com.demo.student_management.exception.BusinessException;
import com.demo.student_management.repository.GiaoVienRepository;
import com.demo.student_management.repository.HocSinhRepository;
import com.demo.student_management.repository.PhanCongGiangDayRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.security.access.AccessDeniedException;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;
import java.util.Set;

@Service
@RequiredArgsConstructor
public class AuthorizationService {

    private static final String ROLE_BAN_QUAN_LY = "BANQUANLY";
    private static final String ROLE_GIAO_VIEN = "GIAOVIEN";

    private final GiaoVienRepository giaoVienRepository;
    private final HocSinhRepository hocSinhRepository;
    private final PhanCongGiangDayRepository phanCongGiangDayRepository;

    public boolean isBanQuanLy() {
        return currentUser()
                .map(user -> ROLE_BAN_QUAN_LY.equalsIgnoreCase(user.loaiTaiKhoan()))
                .orElse(false);
    }

    public Optional<String> getAssignedClassIdForCurrentTeacher() {
        AuthenticatedUser user = requireAuthenticatedUser();
        if (!ROLE_GIAO_VIEN.equalsIgnoreCase(user.loaiTaiKhoan())) {
            return Optional.empty();
        }

        return getCurrentTeacher(user)
                .map(GiaoVien::getLopChuNhiem)
                .map(Lop::getIdLop);
    }

    public Set<String> getAssignedClassIdsForCurrentTeacher() {
        AuthenticatedUser user = requireAuthenticatedUser();
        if (!ROLE_GIAO_VIEN.equalsIgnoreCase(user.loaiTaiKhoan())) {
            return Set.of();
        }

        Set<String> classIds = new java.util.HashSet<>();
        // Add homeroom class
        getAssignedClassIdForCurrentTeacher().ifPresent(classIds::add);

        // Add classes assigned in PHANCONG_GIANGDAY
        List<PhanCongGiangDay> pcList = phanCongGiangDayRepository
                .findByGiaoVien_TaiKhoan_IdTaiKhoan(user.idTaiKhoan());
        for (PhanCongGiangDay pc : pcList) {
            classIds.add(pc.getLop().getIdLop());
        }

        return classIds;
    }

    public void requireAdmin() {
        if (!isBanQuanLy()) {
            throw new AccessDeniedException("Chi Ban quan ly duoc thuc hien chuc nang nay");
        }
    }

    public void requireCanAccessClass(String idLop) {
        AuthenticatedUser user = requireAuthenticatedUser();

        if (ROLE_BAN_QUAN_LY.equalsIgnoreCase(user.loaiTaiKhoan())) {
            return;
        }

        if (ROLE_GIAO_VIEN.equalsIgnoreCase(user.loaiTaiKhoan())) {
            // Check if it is the homeroom class
            boolean isHomeroom = getAssignedClassIdForCurrentTeacher()
                    .map(idLop::equals)
                    .orElse(false);
            if (isHomeroom) {
                return;
            }

            // Check if they are assigned to teach in this class
            boolean hasAssignment = phanCongGiangDayRepository
                    .existsByGiaoVien_TaiKhoan_IdTaiKhoanAndLop_IdLop(user.idTaiKhoan(), idLop);
            if (hasAssignment) {
                return;
            }
        }

        throw new AccessDeniedException("Khong co quyen truy cap lop nay");
    }

    public void requireCanAccessScoreScope(String idLop, String idMonHoc) {
        if (isBanQuanLy()) {
            return;
        }
        AuthenticatedUser user = requireAuthenticatedUser();
        boolean hasAssignment = phanCongGiangDayRepository
                .existsByGiaoVien_TaiKhoan_IdTaiKhoanAndLop_IdLopAndMonHoc_IdMonHoc(
                        user.idTaiKhoan(), idLop, idMonHoc
                );
        if (!hasAssignment) {
            throw new AccessDeniedException("Khong co quyen truy cap diem cho lop va mon hoc nay");
        }
    }

    public void requireCanViewScoreScope(String idLop, String idMonHoc, String idHocKy) {
        if (isBanQuanLy()) {
            return;
        }
        AuthenticatedUser user = requireAuthenticatedUser();

        // 1. Is homeroom teacher for this class?
        boolean isHomeroom = getAssignedClassIdForCurrentTeacher()
                .map(idLop::equals)
                .orElse(false);
        if (isHomeroom) {
            return;
        }

        // 2. Is assigned to teach this subject in this class & term?
        boolean hasAssignment = phanCongGiangDayRepository
                .existsByGiaoVien_TaiKhoan_IdTaiKhoanAndLop_IdLopAndMonHoc_IdMonHocAndHocKy_IdHocKy(
                        user.idTaiKhoan(), idLop, idMonHoc, idHocKy
                );
        if (hasAssignment) {
            return;
        }

        throw new AccessDeniedException("Khong co quyen xem diem cho lop, mon hoc va hoc ky nay");
    }

    public void requireCanEditScoreScope(String idLop, String idMonHoc, String idHocKy) {
        if (isBanQuanLy()) {
            return;
        }
        AuthenticatedUser user = requireAuthenticatedUser();
        boolean hasAssignment = phanCongGiangDayRepository
                .existsByGiaoVien_TaiKhoan_IdTaiKhoanAndLop_IdLopAndMonHoc_IdMonHocAndHocKy_IdHocKy(
                        user.idTaiKhoan(), idLop, idMonHoc, idHocKy
                );
        if (!hasAssignment) {
            throw new AccessDeniedException("Khong co quyen chinh sua diem cho lop, mon hoc va hoc ky nay");
        }
    }

    public void requireCanAccessStudentScore(String idHocSinh, String idMonHoc) {
        HocSinh hocSinh = hocSinhRepository.findById(idHocSinh)
                .orElseThrow(() -> new BusinessException("Khong tim thay hoc sinh"));

        requireCanAccessScoreScope(hocSinh.getLop().getIdLop(), idMonHoc);
    }

    public void requireCanAccessStudentScore(String idHocSinh, String idMonHoc, String idHocKy) {
        HocSinh hocSinh = hocSinhRepository.findById(idHocSinh)
                .orElseThrow(() -> new BusinessException("Khong tim thay hoc sinh"));

        requireCanEditScoreScope(hocSinh.getLop().getIdLop(), idMonHoc, idHocKy);
    }

    private Optional<GiaoVien> getCurrentTeacher(AuthenticatedUser user) {
        return giaoVienRepository.findByTaiKhoan_IdTaiKhoan(user.idTaiKhoan());
    }

    private AuthenticatedUser requireAuthenticatedUser() {
        return currentUser()
                .orElseThrow(() -> new AccessDeniedException("Chua dang nhap"));
    }

    public Optional<AuthenticatedUser> currentUser() {
        Authentication authentication = SecurityContextHolder.getContext().getAuthentication();
        if (authentication == null || !(authentication.getPrincipal() instanceof AuthenticatedUser user)) {
            return Optional.empty();
        }

        return Optional.of(user);
    }
}

