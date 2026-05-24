package com.demo.student_management.service.impl;

import com.demo.student_management.dto.hocsinh.HocSinhResponse;
import com.demo.student_management.dto.lop.LopDetailResponse;
import com.demo.student_management.dto.lop.LopResponse;
import com.demo.student_management.entity.Lop;
import com.demo.student_management.exception.BusinessException;
import com.demo.student_management.repository.HocSinhRepository;
import com.demo.student_management.repository.LopRepository;
import com.demo.student_management.service.LopService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
@RequiredArgsConstructor
public class LopServiceImpl implements LopService {

    private final LopRepository lopRepository;
    private final HocSinhRepository hocSinhRepository;

    @Override
    public List<LopResponse> getAll() {
        return lopRepository.findAll()
                .stream()
                .map(lop -> new LopResponse(
                        lop.getIdLop(),
                        lop.getTenLop(),
                        lop.getSiSo()
                ))
                .toList();
    }

    @Override
    public LopDetailResponse getChiTietLop(String idLop) {
        Lop lop = lopRepository.findById(idLop)
                .orElseThrow(() -> new BusinessException("Không tìm thấy lớp"));

        List<HocSinhResponse> hocSinhs = hocSinhRepository.findByLop_IdLop(idLop)
                .stream()
                .map(hs -> new HocSinhResponse(
                        hs.getIdHocSinh(),
                        hs.getLop().getIdLop(),
                        hs.getTen(),
                        hs.getGioiTinh(),
                        hs.getNgaySinh(),
                        hs.getDiaChi(),
                        hs.getEmail()
                ))
                .toList();

        return new LopDetailResponse(
                lop.getIdLop(),
                lop.getTenLop(),
                lop.getSiSo(),
                hocSinhs
        );
    }
}