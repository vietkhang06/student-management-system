package com.demo.student_management.service.impl;

import com.demo.student_management.dto.hocsinh.HocSinhCreateRequest;
import com.demo.student_management.dto.hocsinh.HocSinhResponse;
import com.demo.student_management.entity.HocSinh;
import com.demo.student_management.entity.Lop;
import com.demo.student_management.exception.BusinessException;
import com.demo.student_management.repository.HocSinhRepository;
import com.demo.student_management.repository.LopRepository;
import com.demo.student_management.service.HocSinhService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import com.demo.student_management.dto.hocsinh.HocSinhSummaryResponse;
import com.demo.student_management.entity.ChiTietDiem;
import com.demo.student_management.repository.ChiTietDiemRepository;

import java.util.Map;
import java.util.stream.Collectors;

import java.time.Period;
import java.util.List;

@Service
@RequiredArgsConstructor
public class HocSinhServiceImpl implements HocSinhService {

    private final HocSinhRepository hocSinhRepository;
    private final LopRepository lopRepository;
    private final ChiTietDiemRepository chiTietDiemRepository;

    @Override
    public HocSinhResponse create(HocSinhCreateRequest request) {
        if (hocSinhRepository.existsById(request.getIdHocSinh())) {
            throw new BusinessException("Mã học sinh đã tồn tại");
        }

        Lop lop = lopRepository.findById(request.getIdLop())
                .orElseThrow(() -> new BusinessException("Lớp không tồn tại"));

        long siSoHienTai = hocSinhRepository.countByLop_IdLop(request.getIdLop());
        if (siSoHienTai >= 40) {
            throw new BusinessException("Lớp đã đủ 40 học sinh");
        }

        int tuoi = Period.between(request.getNgaySinh(), java.time.LocalDate.now()).getYears();
        if (tuoi < 15 || tuoi > 20) {
            throw new BusinessException("Tuổi học sinh phải từ 15 đến 20");
        }

        HocSinh hocSinh = HocSinh.builder()
                .idHocSinh(request.getIdHocSinh())
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

        List<ChiTietDiem> scores =
                chiTietDiemRepository.findAll()
                        .stream()
                        .filter(x ->
                                x.getHocSinh().getIdHocSinh().equals(idHocSinh)
                                        &&
                                        x.getHocKy().getIdHocKy().equals(idHocKy)
                        )
                        .toList();

        if (scores.isEmpty()) {
            return 0.0;
        }

        return scores.stream()
                .filter(x -> x.getDiemTb() != null)
                .collect(Collectors.averagingDouble(
                        x -> x.getDiemTb().doubleValue()
                ));
    }
}