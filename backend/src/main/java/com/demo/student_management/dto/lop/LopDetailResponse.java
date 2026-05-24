package com.demo.student_management.dto.lop;

import com.demo.student_management.dto.hocsinh.HocSinhResponse;
import lombok.AllArgsConstructor;
import lombok.Data;

import java.util.List;

@Data
@AllArgsConstructor
public class LopDetailResponse {
    private String idLop;
    private String tenLop;
    private Integer siSo;
    private List<HocSinhResponse> hocSinhs;
}