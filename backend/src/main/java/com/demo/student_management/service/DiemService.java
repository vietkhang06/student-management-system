package com.demo.student_management.service;

import com.demo.student_management.dto.diem.DiemCreateRequest;
import com.demo.student_management.dto.diem.DiemResponse;

import java.util.List;

public interface DiemService {
    DiemResponse save(DiemCreateRequest request);
    List<DiemResponse> getByClassSubjectTerm(String idLop, String idMonHoc, String idHocKy);
}