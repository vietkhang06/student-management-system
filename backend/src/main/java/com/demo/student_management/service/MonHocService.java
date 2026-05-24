package com.demo.student_management.service;

import com.demo.student_management.dto.monhoc.MonHocResponse;
import java.util.List;

public interface MonHocService {
    List<MonHocResponse> getAll();
}
