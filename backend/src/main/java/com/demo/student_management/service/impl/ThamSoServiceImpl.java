package com.demo.student_management.service.impl;

import com.demo.student_management.dto.thamso.ThamSoResponse;
import com.demo.student_management.dto.thamso.ThamSoUpdateRequest;
import com.demo.student_management.entity.ThamSo;
import com.demo.student_management.exception.BusinessException;
import com.demo.student_management.repository.ThamSoRepository;
import com.demo.student_management.service.ThamSoService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import java.util.List;

@Service
@RequiredArgsConstructor
public class ThamSoServiceImpl implements ThamSoService {

    private final ThamSoRepository thamSoRepository;

    @Override
    public List<ThamSoResponse> getAll() {
        return thamSoRepository.findAll().stream()
                .map(ts -> new ThamSoResponse(ts.getIdThamSo(), ts.getTenThamSo(), ts.getKieuThamSo(), ts.getGiaTriThamSo()))
                .toList();
    }

    @Override
    public ThamSoResponse getById(String idThamSo) {
        ThamSo ts = thamSoRepository.findById(idThamSo)
                .orElseThrow(() -> new BusinessException("Không tìm thấy tham số"));
        return new ThamSoResponse(ts.getIdThamSo(), ts.getTenThamSo(), ts.getKieuThamSo(), ts.getGiaTriThamSo());
    }

    @Override
    public ThamSoResponse update(String idThamSo, ThamSoUpdateRequest request) {
        ThamSo ts = thamSoRepository.findById(idThamSo)
                .orElseThrow(() -> new BusinessException("Không tìm thấy tham số"));
        
        ts.setGiaTriThamSo(request.getGiaTriThamSo());
        thamSoRepository.save(ts);
        
        return new ThamSoResponse(ts.getIdThamSo(), ts.getTenThamSo(), ts.getKieuThamSo(), ts.getGiaTriThamSo());
    }
}
