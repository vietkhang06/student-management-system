package com.demo.student_management.service;

import com.demo.student_management.dto.lop.LopDetailResponse;
import com.demo.student_management.dto.lop.LopResponse;

import java.util.List;

public interface LopService {
    List<LopResponse> getAll();
    LopDetailResponse getChiTietLop(String idLop);
}