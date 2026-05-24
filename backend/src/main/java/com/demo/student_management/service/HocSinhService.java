package com.demo.student_management.service;

import com.demo.student_management.dto.hocsinh.HocSinhCreateRequest;
import com.demo.student_management.dto.hocsinh.HocSinhResponse;
import com.demo.student_management.dto.hocsinh.HocSinhSummaryResponse;

import java.util.List;

public interface HocSinhService {
    HocSinhResponse create(HocSinhCreateRequest request);
    List<HocSinhResponse> getAll();
    HocSinhResponse getById(String idHocSinh);
    List<HocSinhResponse> searchByName(String ten);
    List<HocSinhSummaryResponse> getStudentSummaries();
}