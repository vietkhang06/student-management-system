package com.demo.student_management.service;

import com.demo.student_management.dto.thamso.ThamSoResponse;
import com.demo.student_management.dto.thamso.ThamSoUpdateRequest;
import java.util.List;

public interface ThamSoService {
    List<ThamSoResponse> getAll();
    ThamSoResponse getById(String idThamSo);
    ThamSoResponse update(String idThamSo, ThamSoUpdateRequest request);
}
