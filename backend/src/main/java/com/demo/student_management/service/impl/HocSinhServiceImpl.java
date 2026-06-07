package com.demo.student_management.service.impl;

import com.demo.student_management.dto.hocsinh.HocSinhCreateRequest;
import com.demo.student_management.dto.hocsinh.HocSinhResponse;
import com.demo.student_management.dto.hocsinh.HocSinhUpdateRequest;
import com.demo.student_management.entity.HocSinh;
import com.demo.student_management.entity.Lop;
import com.demo.student_management.exception.BusinessException;
import com.demo.student_management.repository.HocSinhRepository;
import com.demo.student_management.repository.LopRepository;
import com.demo.student_management.service.HocSinhService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.demo.student_management.dto.hocsinh.HocSinhSummaryResponse;
import com.demo.student_management.entity.ChiTietDiem;
import com.demo.student_management.repository.ChiTietDiemRepository;

import java.util.Map;
import java.util.stream.Collectors;

import java.time.Period;
import java.util.List;

import com.demo.student_management.entity.ThamSo;
import com.demo.student_management.repository.ThamSoRepository;
import com.demo.student_management.service.LichSuHeThongService;


@Service
@RequiredArgsConstructor
public class HocSinhServiceImpl implements HocSinhService {

    private final HocSinhRepository hocSinhRepository;
    private final LopRepository lopRepository;
    private final ChiTietDiemRepository chiTietDiemRepository;
    private final ThamSoRepository thamSoRepository;
    private final LichSuHeThongService lichSuHeThongService;

    @Override
    @Transactional
    public HocSinhResponse create(HocSinhCreateRequest request) {
        String idHocSinh = request.getIdHocSinh();
        if (idHocSinh == null || idHocSinh.trim().isEmpty()) {
            idHocSinh = generateNextId();
        } else {
            idHocSinh = idHocSinh.trim();
        }

        if (hocSinhRepository.existsById(idHocSinh)) {
            throw new BusinessException("Mã học sinh đã tồn tại");
        }

        if (request.getNgaySinh() == null) {
            throw new BusinessException("Ngày sinh không được để trống");
        }

        Lop lop = lopRepository.findById(request.getIdLop())
                .orElseThrow(() -> new BusinessException("Lớp không tồn tại"));

        long siSoHienTai = hocSinhRepository.countByLop_IdLop(request.getIdLop());
        int maxSiSo = Integer.parseInt(thamSoRepository.findByTenThamSo("QD2_SI_SO_TOI_DA")
                .map(ThamSo::getGiaTriThamSo)
                .orElse("40"));
        if (siSoHienTai >= maxSiSo) {
            throw new BusinessException("Lớp đã đủ " + maxSiSo + " học sinh");
        }

        int minTuoi = Integer.parseInt(thamSoRepository.findByTenThamSo("QD1_MIN_TUOI")
                .map(ThamSo::getGiaTriThamSo)
                .orElse("15"));
        int maxTuoi = Integer.parseInt(thamSoRepository.findByTenThamSo("QD1_MAX_TUOI")
                .map(ThamSo::getGiaTriThamSo)
                .orElse("20"));

        int tuoi = Period.between(request.getNgaySinh(), java.time.LocalDate.now()).getYears();
        if (tuoi < minTuoi || tuoi > maxTuoi) {
            throw new BusinessException("Tuổi học sinh phải từ " + minTuoi + " đến " + maxTuoi);
        }

        validateEmail(request.getEmail());

        HocSinh hocSinh = HocSinh.builder()
                .idHocSinh(idHocSinh)
                .lop(lop)
                .ten(request.getTen())
                .gioiTinh(request.getGioiTinh())
                .ngaySinh(request.getNgaySinh())
                .diaChi(request.getDiaChi())
                .email(request.getEmail())
                .build();

        hocSinhRepository.save(hocSinh);

        lop.setSiSo((int) (siSoHienTai + 1));
        lopRepository.save(lop);

        lichSuHeThongService.log("Thêm học sinh", String.format("Thêm học sinh [%s - %s] vào lớp [%s]", hocSinh.getIdHocSinh(), hocSinh.getTen(), lop.getTenLop()));

        return toResponse(hocSinh);
    }

    @Override
    public List<HocSinhResponse> getAll() {
        return hocSinhRepository.findAll()
                .stream()
                .map(this::toResponse)
                .toList();
    }

    @Override
    public HocSinhResponse getById(String idHocSinh) {
        HocSinh hocSinh = hocSinhRepository.findById(idHocSinh)
                .orElseThrow(() -> new BusinessException("Không tìm thấy học sinh"));
        return toResponse(hocSinh);
    }

    @Override
    public List<HocSinhResponse> searchByName(String ten) {
        return hocSinhRepository.findByTenContainingIgnoreCase(ten)
                .stream()
                .map(this::toResponse)
                .toList();
    }

    private HocSinhResponse toResponse(HocSinh hs) {
        return new HocSinhResponse(
                hs.getIdHocSinh(),
                hs.getLop().getIdLop(),
                hs.getTen(),
                hs.getGioiTinh(),
                hs.getNgaySinh(),
                hs.getDiaChi(),
                hs.getEmail()
        );
    }

    @Override
    public List<HocSinhSummaryResponse> getStudentSummaries() {

        List<HocSinh> hocSinhs = hocSinhRepository.findAll();

        return hocSinhs.stream()
                .map(hs -> {

                    Double tbHK1 = calculateSemesterAverage(
                            hs.getIdHocSinh(),
                            "HKI"
                    );

                    Double tbHK2 = calculateSemesterAverage(
                            hs.getIdHocSinh(),
                            "HKII"
                    );

                    return new HocSinhSummaryResponse(
                            hs.getIdHocSinh(),
                            hs.getTen(),
                            hs.getLop().getIdLop(),
                            tbHK1,
                            tbHK2
                    );
                })
                .toList();
    }

    private Double calculateSemesterAverage(
            String idHocSinh,
            String idHocKy
    ) {

        List<ChiTietDiem> scores = chiTietDiemRepository
                .findByHocSinh_IdHocSinhAndHocKy_IdHocKy(idHocSinh, idHocKy);

        if (scores.isEmpty()) {
            return 0.0;
        }

        return scores.stream()
                .filter(x -> x.getDiemTb() != null)
                .collect(Collectors.averagingDouble(
                        x -> x.getDiemTb().doubleValue()
                ));
    }

    @Override
    @Transactional
    public HocSinhResponse update(String idHocSinh, HocSinhUpdateRequest request) {
        HocSinh hocSinh = hocSinhRepository.findById(idHocSinh)
                .orElseThrow(() -> new BusinessException("Không tìm thấy học sinh"));

        if (request.getNgaySinh() == null) {
            throw new BusinessException("Ngày sinh không được để trống");
        }

        int minTuoi = Integer.parseInt(thamSoRepository.findByTenThamSo("QD1_MIN_TUOI")
                .map(ThamSo::getGiaTriThamSo)
                .orElse("15"));
        int maxTuoi = Integer.parseInt(thamSoRepository.findByTenThamSo("QD1_MAX_TUOI")
                .map(ThamSo::getGiaTriThamSo)
                .orElse("20"));

        int tuoi = Period.between(request.getNgaySinh(), java.time.LocalDate.now()).getYears();
        if (tuoi < minTuoi || tuoi > maxTuoi) {
            throw new BusinessException("Tuổi học sinh phải từ " + minTuoi + " đến " + maxTuoi);
        }

        validateEmail(request.getEmail());

        Lop oldLop = hocSinh.getLop();
        Lop newLop = lopRepository.findById(request.getIdLop())
                .orElseThrow(() -> new BusinessException("Lớp không tồn tại"));

        if (!oldLop.getIdLop().equals(newLop.getIdLop())) {
            long siSoHienTai = hocSinhRepository.countByLop_IdLop(newLop.getIdLop());
            int maxSiSo = Integer.parseInt(thamSoRepository.findByTenThamSo("QD2_SI_SO_TOI_DA")
                    .map(ThamSo::getGiaTriThamSo)
                    .orElse("40"));
            if (siSoHienTai >= maxSiSo) {
                throw new BusinessException("Lớp mới đã đủ " + maxSiSo + " học sinh");
            }
            hocSinh.setLop(newLop);
        }

        StringBuilder details = new StringBuilder(String.format("Sửa học sinh [%s - %s]. ", hocSinh.getIdHocSinh(), request.getTen()));
        if (!oldLop.getIdLop().equals(newLop.getIdLop())) {
            details.append(String.format("Chuyển lớp: %s -> %s. ", oldLop.getTenLop(), newLop.getTenLop()));
        }
        if (!hocSinh.getTen().equals(request.getTen())) {
            details.append(String.format("Họ tên: %s -> %s. ", hocSinh.getTen(), request.getTen()));
        }
        if (!hocSinh.getGioiTinh().equals(request.getGioiTinh())) {
            details.append(String.format("Giới tính: %s -> %s. ", hocSinh.getGioiTinh(), request.getGioiTinh()));
        }
        if (!hocSinh.getNgaySinh().equals(request.getNgaySinh())) {
            details.append(String.format("Ngày sinh: %s -> %s. ", hocSinh.getNgaySinh(), request.getNgaySinh()));
        }
        if (!hocSinh.getDiaChi().equals(request.getDiaChi())) {
            details.append(String.format("Địa chỉ: %s -> %s. ", hocSinh.getDiaChi(), request.getDiaChi()));
        }
        if ((hocSinh.getEmail() != null && !hocSinh.getEmail().equals(request.getEmail())) || (hocSinh.getEmail() == null && request.getEmail() != null)) {
            details.append(String.format("Email: %s -> %s. ", hocSinh.getEmail(), request.getEmail()));
        }

        hocSinh.setTen(request.getTen());
        hocSinh.setGioiTinh(request.getGioiTinh());
        hocSinh.setNgaySinh(request.getNgaySinh());
        hocSinh.setDiaChi(request.getDiaChi());
        hocSinh.setEmail(request.getEmail());

        hocSinhRepository.save(hocSinh);

        // Recount sizes
        oldLop.setSiSo((int) hocSinhRepository.countByLop_IdLop(oldLop.getIdLop()));
        lopRepository.save(oldLop);

        if (!oldLop.getIdLop().equals(newLop.getIdLop())) {
            newLop.setSiSo((int) hocSinhRepository.countByLop_IdLop(newLop.getIdLop()));
            lopRepository.save(newLop);
        }

        lichSuHeThongService.log("Sửa học sinh", details.toString());

        return toResponse(hocSinh);
    }

    @Override
    @Transactional
    public void delete(String idHocSinh) {
        HocSinh hocSinh = hocSinhRepository.findById(idHocSinh)
                .orElseThrow(() -> new BusinessException("Không tìm thấy học sinh"));

        Lop lop = hocSinh.getLop();

        // 1. Delete all related scores in ChiTietDiem
        chiTietDiemRepository.deleteByHocSinh_IdHocSinh(idHocSinh);

        // 2. Delete the student
        hocSinhRepository.delete(hocSinh);

        // 3. Update class size
        if (lop != null) {
            lop.setSiSo((int) hocSinhRepository.countByLop_IdLop(lop.getIdLop()));
            lopRepository.save(lop);
        }

        lichSuHeThongService.log("Xóa học sinh", String.format("Xóa học sinh [%s - %s] khỏi lớp [%s]", hocSinh.getIdHocSinh(), hocSinh.getTen(), lop != null ? lop.getTenLop() : "Không"));
    }

    @Override
    public boolean hasScores(String idHocSinh) {
        return chiTietDiemRepository.existsByHocSinh_IdHocSinh(idHocSinh);
    }

    private void validateEmail(String email) {
        if (email != null && !email.trim().isEmpty()) {
            String trimmed = email.trim();
            if (!trimmed.matches("^[a-zA-Z0-9_+&*-]+(?:\\.[a-zA-Z0-9_+&*-]+)*@(?:[a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,7}$")) {
                throw new BusinessException("Email không hợp lệ");
            }
        }
    }

    private synchronized String generateNextId() {
        List<HocSinh> list = hocSinhRepository.findAll();
        int maxNum = 0;
        for (HocSinh hs : list) {
            String id = hs.getIdHocSinh();
            if (id != null && id.toUpperCase().startsWith("HS")) {
                try {
                    int num = Integer.parseInt(id.substring(2).trim());
                    if (num > maxNum) {
                        maxNum = num;
                    }
                } catch (NumberFormatException e) {
                    // Ignore invalid suffix
                }
            }
        }
        int nextNum = maxNum + 1;
        return String.format("HS%03d", nextNum);
    }
}