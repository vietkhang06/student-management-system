package com.demo.student_management.service.impl;

import com.demo.student_management.dto.monhoc.MonHocResponse;
import com.demo.student_management.repository.MonHocRepository;
import com.demo.student_management.service.MonHocService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import java.util.List;

@Service
@RequiredArgsConstructor
public class MonHocServiceImpl implements MonHocService {

    private final MonHocRepository monHocRepository;

    @Override
    public List<MonHocResponse> getAll() {
        return monHocRepository.findAll().stream()
                .map(mh -> new MonHocResponse(mh.getIdMonHoc(), mh.getTenMonHoc()))
                .toList();
    }
}
