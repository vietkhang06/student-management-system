package com.demo.student_management.service.impl;

import com.demo.student_management.dto.hocky.HocKyResponse;
import com.demo.student_management.repository.HocKyRepository;
import com.demo.student_management.service.HocKyService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import java.util.List;

@Service
@RequiredArgsConstructor
public class HocKyServiceImpl implements HocKyService {

    private final HocKyRepository hocKyRepository;

    @Override
    public List<HocKyResponse> getAll() {
        return hocKyRepository.findAll().stream()
                .map(hk -> new HocKyResponse(hk.getIdHocKy(), hk.getTenHocKy(), hk.getNamHoc()))
                .toList();
    }
}
