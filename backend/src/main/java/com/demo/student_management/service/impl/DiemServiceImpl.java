package com.demo.student_management.service.impl;

import com.demo.student_management.dto.diem.DiemCreateRequest;
import com.demo.student_management.dto.diem.DiemResponse;
import com.demo.student_management.entity.*;
import com.demo.student_management.exception.BusinessException;
import com.demo.student_management.repository.*;
import com.demo.student_management.service.DiemService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import com.demo.student_management.service.LichSuHeThongService;

import java.math.BigDecimal;
import java.math.RoundingMode;
import java.util.List;

import org.springframework.transaction.annotation.Transactional;

@Service
@RequiredArgsConstructor
public class DiemServiceImpl implements DiemService {

    private final ChiTietDiemRepository chiTietDiemRepository;
    private final HocSinhRepository hocSinhRepository;
    private final MonHocRepository monHocRepository;
    private final HocKyRepository hocKyRepository;
    private final LichSuHeThongService lichSuHeThongService;

    @Override
    public DiemResponse save(DiemCreateRequest request) {
        validateScore(request.getDiem15(), "Điểm thường xuyên");
        validateScore(request.getDiem45(), "Điểm một tiết");
        validateScore(request.getDiemCk(), "Điểm cuối kỳ");

        HocSinh hocSinh = hocSinhRepository.findById(request.getIdHocSinh())
                .orElseThrow(() -> new BusinessException("Không tìm thấy học sinh"));

        MonHoc monHoc = monHocRepository.findById(request.getIdMonHoc())
                .orElseThrow(() -> new BusinessException("Không tìm thấy môn học"));

        HocKy hocKy = hocKyRepository.findById(request.getIdHocKy())
                .orElseThrow(() -> new BusinessException("Không tìm thấy học kỳ"));

        BigDecimal diemTb = request.getDiem15()
                .add(request.getDiem45().multiply(BigDecimal.valueOf(2)))
                .add(request.getDiemCk().multiply(BigDecimal.valueOf(3)))
                .divide(BigDecimal.valueOf(6), 2, RoundingMode.HALF_UP);

        ChiTietDiemId id = new ChiTietDiemId(
                request.getIdHocSinh(),
                request.getIdMonHoc(),
                request.getIdHocKy()
        );

        java.util.Optional<ChiTietDiem> existingOpt = chiTietDiemRepository.findById(id);
        String details;
        if (existingOpt.isPresent()) {
            ChiTietDiem existing = existingOpt.get();
            details = String.format("Cập nhật điểm môn [%s] học kỳ [%s] cho học sinh [%s - %s]. Chi tiết: Thường xuyên: %s -> %s, Một tiết: %s -> %s, Cuối kỳ: %s -> %s",
                    monHoc.getTenMonHoc(),
                    hocKy.getTenHocKy(),
                    hocSinh.getIdHocSinh(),
                    hocSinh.getTen(),
                    existing.getDiem15() != null ? existing.getDiem15() : "Chưa có",
                    request.getDiem15(),
                    existing.getDiem45() != null ? existing.getDiem45() : "Chưa có",
                    request.getDiem45(),
                    existing.getDiemCk() != null ? existing.getDiemCk() : "Chưa có",
                    request.getDiemCk()
            );
        } else {
            details = String.format("Nhập mới điểm môn [%s] học kỳ [%s] cho học sinh [%s - %s]. Chi tiết: Thường xuyên: %s, Một tiết: %s, Cuối kỳ: %s",
                    monHoc.getTenMonHoc(),
                    hocKy.getTenHocKy(),
                    hocSinh.getIdHocSinh(),
                    hocSinh.getTen(),
                    request.getDiem15(),
                    request.getDiem45(),
                    request.getDiemCk()
            );
        }

        ChiTietDiem entity = ChiTietDiem.builder()
                .id(id)
                .hocSinh(hocSinh)
                .monHoc(monHoc)
                .hocKy(hocKy)
                .diem15(request.getDiem15())
                .diem45(request.getDiem45())
                .diemCk(request.getDiemCk())
                .diemTb(diemTb)
                .build();

        chiTietDiemRepository.save(entity);

        lichSuHeThongService.log("Cập nhật điểm", details);

        return new DiemResponse(
                request.getIdHocSinh(),
                hocSinh.getTen(),
                request.getIdMonHoc(),
                request.getIdHocKy(),
                request.getDiem15(),
                request.getDiem45(),
                request.getDiemCk(),
                diemTb
        );
    }

    @Override
    @Transactional(readOnly = true)
    public List<DiemResponse> getByClassSubjectTerm(String idLop, String idMonHoc, String idHocKy) {
        List<HocSinh> students = hocSinhRepository.findByLop_IdLop(idLop);
        List<ChiTietDiem> scores = chiTietDiemRepository
                .findByHocSinh_Lop_IdLopAndMonHoc_IdMonHocAndHocKy_IdHocKy(idLop, idMonHoc, idHocKy);

        java.util.Map<String, ChiTietDiem> scoreMap = scores.stream()
                .collect(java.util.stream.Collectors.toMap(
                        x -> x.getHocSinh().getIdHocSinh(),
                        x -> x
                ));

        return students.stream()
                .map(student -> {
                    ChiTietDiem score = scoreMap.get(student.getIdHocSinh());
                    if (score != null) {
                        return new DiemResponse(
                                student.getIdHocSinh(),
                                student.getTen(),
                                idMonHoc,
                                idHocKy,
                                score.getDiem15(),
                                score.getDiem45(),
                                score.getDiemCk(),
                                score.getDiemTb()
                        );
                    } else {
                        return new DiemResponse(
                                student.getIdHocSinh(),
                                student.getTen(),
                                idMonHoc,
                                idHocKy,
                                null,
                                null,
                                null,
                                null
                        );
                    }
                })
                .toList();
    }

    private void validateScore(BigDecimal score, String fieldName) {
        if (score == null) {
            throw new BusinessException(fieldName + " không được để trống");
        }
        if (score.compareTo(BigDecimal.ZERO) < 0 || score.compareTo(BigDecimal.TEN) > 0) {
            throw new BusinessException(fieldName + " phải từ 0 đến 10");
        }
    }
}