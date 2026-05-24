package com.demo.student_management.service;

import com.demo.student_management.dto.hocky.HocKyResponse;
import java.util.List;

public interface HocKyService {
    List<HocKyResponse> getAll();
}
